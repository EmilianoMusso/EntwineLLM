### v1.13.0 Authentication token for LLM shared use
---
- I't now possibile to specify an authentication token for LLM using through nginx

### v1.12.0 Added LMStudio support
---
- It is now possibile to switch between Ollama and LMStudio
- Minor refactoring

### v1.11.0 Added answer language
---
- LLMs are now selectable by querying Ollama APIs
- Markdown views can now use follow-up functions
- Minor refactoring

### v1.10.0 Added answer language
---
- It is now possibile to set a language in which the LLM must answer
- Refactoring for LLM httpClient

### v1.9.0 Generic refactoring
---
- Main menu moved to Extensions menu; command are now grouped
- Added multiple programming languages support
- Minor refactorings

### v1.8.0 Added specific models for each operation
---
- User can now configure a specific LLM to serve each operation

### v1.7.0 Document and Code review commands
---
Added new commands to:
- Generate documentation for selected code
- Review selected code to identify issues and improvements

### v1.6.0 Follow-up prompt
---
- Visual style revisited
- General refactor
- Implemented command to save the generated code in a `.cs` file, added to the current project
- Implemented follow-up prompt to better define LLM response (a91501c)

### v1.5.0 Unit tests generation
---
- General refactor; implemented unit tests generation (fc8af9b)

### v1.0.0 First release
---
- First release; code writing and refactor functions enabled (02d319f)