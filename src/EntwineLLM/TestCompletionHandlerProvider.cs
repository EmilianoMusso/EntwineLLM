using EntwineLlm.Clients.Interfacs;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace EntwineLlm
{
    internal class TestCompletionSource : ICompletionSource
    {
        private TestCompletionSourceProvider m_sourceProvider;
        private ITextBuffer m_textBuffer;
        private readonly ILlmClient _llmClient;
        private bool isDisposed;

        public TestCompletionSource(TestCompletionSourceProvider sourceProvider, ITextBuffer textBuffer)
        {
            m_sourceProvider = sourceProvider;
            m_textBuffer = textBuffer;
            _llmClient = EntwineLlmPackage.LlmClient;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            var triggerPoint = session.GetTriggerPoint(m_textBuffer.CurrentSnapshot);
            if (triggerPoint == null) return;

            var currentSnapshot = m_textBuffer.CurrentSnapshot;
            var span = new SnapshotSpan(triggerPoint.Value, 0);
            var currentText = span.GetText();

            var suggestions = EntwineLlmPackage.LlmClient.GetCodeSuggestionsAsync(Enums.CodeType.Manual, currentText).GetAwaiter().GetResult();

            var completions = new List<Completion>();
            var suggestion = suggestions.Code;
            {
                completions.Add(new Completion(suggestion, suggestion, suggestion, null, null));
            }

            completionSets.Add(new CompletionSet(
                "LLM Suggestions",
                "Suggestions from Entwine LLM",
                FindTokenSpanAtPosition(session.GetTriggerPoint(m_textBuffer), session),
                completions,
                null));
        }

        private ITrackingSpan FindTokenSpanAtPosition(ITrackingPoint triggerPoint, ICompletionSession session)
        {
            SnapshotPoint currentPoint = triggerPoint.GetPoint(m_textBuffer.CurrentSnapshot);
            ITextStructureNavigator navigator = m_sourceProvider.NavigatorService.GetTextStructureNavigator(m_textBuffer);
            TextExtent extent = navigator.GetExtentOfWord(currentPoint);
            return currentPoint.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
        }


        public void Dispose()
        {
            if (!this.isDisposed)
            {
                GC.SuppressFinalize(this);
                this.isDisposed = true;
            }
        }
    }

    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("plaintext")]
    [Name("token completion")]
    internal class TestCompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Setter used by MEF.")]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new TestCompletionSource(this, textBuffer);
        }
    }

    [Export(typeof(IVsTextViewCreationListener))]
    [Name("token completion handler")]
    [ContentType("plaintext")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal class TestCompletionHandlerProvider : IVsTextViewCreationListener
    {
        [Import]
        internal IVsEditorAdaptersFactoryService AdapterService = null;
   
        [Import]
        internal ICompletionBroker CompletionBroker { get; set; }
       
        [Import]
        internal SVsServiceProvider ServiceProvider { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = this.AdapterService.GetWpfTextView(textViewAdapter);
            if (textView == null)
            {
                return;
            }

            Func<TestCompletionCommandHandler> createCommandHandler = () => new TestCompletionCommandHandler(textViewAdapter, textView, this);
            textView.Properties.GetOrCreateSingletonProperty(createCommandHandler);
        }
    }

    internal class TestCompletionCommandHandler : IOleCommandTarget
    {
        private IOleCommandTarget nextCommandHandler;
        private ITextView textView;
        private TestCompletionHandlerProvider provider;
        private ICompletionSession session;

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.VisualStudio.TextManager.Interop.IVsTextView.AddCommandFilter(Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget,Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget@)", Justification = "If it fails, it fails :).")]
        internal TestCompletionCommandHandler(IVsTextView textViewAdapter, ITextView textView, TestCompletionHandlerProvider provider)
        {
            this.textView = textView;
            this.provider = provider;

            textViewAdapter.AddCommandFilter(this, out nextCommandHandler);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (VsShellUtilities.IsInAutomationFunction(this.provider.ServiceProvider))
            {
                return this.nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            }

            // make a copy of this so we can look at it after forwarding some commands
            uint commandID = nCmdID;
            char typedChar = char.MinValue;

            // make sure the input is a char before getting it
            if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
            {
                typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
            }

            // check for a commit character
            if (nCmdID == (uint)VSConstants.VSStd2KCmdID.RETURN
                || nCmdID == (uint)VSConstants.VSStd2KCmdID.TAB)
            {
                // check for a a selection
                if (this.session != null && !this.session.IsDismissed)
                {
                    // if the selection is fully selected, commit the current session
                    if (this.session.SelectedCompletionSet.SelectionStatus.IsSelected)
                    {
                        this.session.Commit();

                        // also, don't add the character to the buffer
                        return VSConstants.S_OK;
                    }
                    else
                    {
                        // if there is no selection, dismiss the session
                        this.session.Dismiss();
                    }
                }
            }

            // pass along the command so the char is added to the buffer
            int retVal = this.nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            bool handled = false;
            if (!typedChar.Equals(char.MinValue) && char.IsLetterOrDigit(typedChar))
            {
                if (this.session == null || this.session.IsDismissed)
                {
                    // If there is no active session, bring up completion
                    this.TriggerCompletion();
                }
                else
                {
                    // the completion session is already active, so just filter
                    this.session.Recalculate();
                    this.session.Filter();
                }

                handled = true;
            }
            else if (commandID == (uint)VSConstants.VSStd2KCmdID.BACKSPACE
                || commandID == (uint)VSConstants.VSStd2KCmdID.DELETE)
            {
                // redo the filter if there is a deletion
                if (this.session != null && !this.session.IsDismissed)
                {
                    this.session.Recalculate();
                    this.session.Filter();
                }

                handled = true;
            }

            if (handled)
            {
                return VSConstants.S_OK;
            }

            return retVal;
        }

        private bool TriggerCompletion()
        {
            // the caret must be in a non-projection location
            SnapshotPoint? caretPoint =
            this.textView.Caret.Position.Point.GetPoint(
            textBuffer => (!textBuffer.ContentType.IsOfType("projection")), PositionAffinity.Predecessor);
            if (!caretPoint.HasValue)
            {
                return false;
            }

            this.session = this.provider.CompletionBroker.CreateCompletionSession(
                this.textView,
                caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position, PointTrackingMode.Positive),
                true);

            // subscribe to the Dismissed event on the session
            this.session.Dismissed += this.OnSessionDismissed;
            this.session.Start();

            return true;
        }

        private void OnSessionDismissed(object sender, EventArgs e)
        {
            session.Dismissed -= this.OnSessionDismissed;
            session = null;
        }
    }
}
