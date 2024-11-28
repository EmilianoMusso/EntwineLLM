## EntwineLLM
### LLM coding assistant extension for Visual Studio

<img src="./src/EntwineLLM/Resources/entwine-logo.jpg" width="80" height="80" style="float:left;margin-right:10px"/>
EntwineLLM is a free Visual Studio extension designed to leverage LLM capabilities to assist developers in writing code, without relying on third-party APIs. Instead, it uses an open LLM implementation installed locally on the user's PC, such as Ollama

#### Prerequisites

To use the EntwineLLM extension, you need to have a local or Docker-hosted open LLM implementation, such as Ollama. The LLM must be running and expose an accessible API endpoint that the extension can connect to. Ensure that the API is properly configured and reachable from within Visual Studio to enable seamless integration with the extension.

*Resources*:
* [Ollama download for Windows](https://ollama.com/download/windows)
* [Ollama models](https://ollama.com/search)

#### Setup
After installing the EntwineLLM extension, its configuration options will be available in the Visual Studio Options menu. These options allow users to specify the base URL of the locally installed LLM, select the LLM model to use (after it has been installed), and configure the HTTP request timeout settings for communication with the LLM. These settings provide flexibility in customizing the behavior of the extension to match the user’s environment and preferences.

![image](./src/EntwineLLM/Resources/vs-entwine-options.png)

#### Using the extension
After installing the extension, the following commands will be available in the Tools menu:

* `Ask Entwine`: Select a block of code or text related to the function you wish to create, and the extension will call the LLM APIs available at the URL configured in the options. The extension will then present a window displaying the suggested code. If the user chooses to apply the suggestion, the new code will overwrite the originally selected text or code, seamlessly integrating the AI's assistance into the developer's workflow.
* `Generate Unit Tests with Entwine`: Select a block of code or function, and this command will generate unit tests covering all paths for the selection. The extension queries the LLM and displays the results in a separate window. If accepted, the generated tests can be applied directly to the codebase.

All the available functions can overwrite the originally selected code with the generate one by pressing the `Apply` button, or save it in a dinamically named file, which will be persisted on disk and under the current project folder, by pressing `Save` button. This allows users to inspect and integrate the new code into their workflow at their discretion.

![image](./src/EntwineLLM/Resources/vs-entwine-suggestion.png)

#### How it works
The prompt for this extension is designed to focus specifically on C# development within the Microsoft .NET ecosystem, including the full framework, .NET Core, and related technologies like LINQ, Entity Framework, and ASP.NET Core. It has a strict set of rules: requests unrelated to C# coding are rejected. If the request involves refactoring code, it follows Clean Code principles, ensuring readability, maintainability, and performance, with no extra explanations or comments provided. For new code requests, the same principles apply, with the emphasis on modularity, testability, and high performance. All code is provided in raw C# format, following strict style guidelines (Allman-style braces, vertical slicing) with no comments or additional context

![image](./src/EntwineLLM/Resources/vs-entwine-menu.png)

#### Why Entwine?
The name Entwine and its logo, resembling the helix of DNA, symbolize the union of two forces: the natural intelligence and skills of the developer, and the artificial intelligence provided by the LLM. Just as DNA represents the intricate intertwining of biological elements that form life, Entwine reflects the harmonious connection between human creativity and AI-driven assistance. This synergy allows developers to harness the power of LLMs, enhancing their coding process while retaining their unique problem-solving abilities, creating a seamless collaboration between human and machine.