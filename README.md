# Libraries

`Libraries.sln` is a C#/.NET solution containing reusable libraries plus a supporting configuration tool.

`Library.Operations` is the centerpiece of the repository and the primary domain-focused library.

## Projects

| Project | README | One-line description |
|---|---|---|
| `Library.Operations` | [Library.Operations/README.md](Library.Operations/README.md) | Outcome/result and error-building primitives for operational workflows. |
| `Library.Authentication` | [Library.Authentication/README.md](Library.Authentication/README.md) | Authentication-focused models and services including JWT generation and credential token utilities. |
| `Tools.Configuration.Reader` | [Tools/Tools.Configuration.Reader/README.md](Tools/Tools.Configuration.Reader/README.md) | Command-line tool that reads JSON config and publishes parameters to AWS Systems Manager Parameter Store. |

## Build & CI

Build and workflow behavior is documented in [docs/BUILD.md](docs/BUILD.md).
