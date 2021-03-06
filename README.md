# Docker CLI Wrapper

A C# wrapper over the Docker CLI. Unlike [Docker.DotNet](https://github.com/Microsoft/Docker.DotNet), DockerCLIWrapper issues shell commands and parses the Standard Output and Error, therefore can only be used locally. [Docker.DotNet](https://github.com/Microsoft/Docker.DotNet) is still the preferred tool but DockerCLIWrapper will eventually (and hopefully) offer all the CLI features, which will (also hopefully) be used for a VS extension which is in the works.

## Usage

Before you use the library you need to make sure that Docker is running locally and that it is in your PATH. The current version has been tested against:

`Client:`  
` Version:       17.12.0-ce`  
` API version:   1.35`  
` Go version:    go1.9.2`  
` Git commit:    c97c6d6`  
` Built: Wed Dec 27 20:05:22 2017`  
` OS/Arch:       windows/amd64`  

`Server:`  
` Engine:`  
`  Version:      17.12.0-ce`  
`  API version:  1.35 (minimum version 1.24)`  
`  Go version:   go1.9.2`  
`  Git commit:   c97c6d6`  
`  Built:        Wed Dec 27 20:15:52 2017`  
`  OS/Arch:      windows/amd64`  
`  Experimental: true`  
  
The wrapper uses fluent syntax where possible and is fully asnyc. At the time of writing, only the following functionality is available:

### Examples

Get the running status and details (as above) of both the client & server:

`await new DockerStatus();`

Return a list of all images currently installed and do not truncate the data:

`await new DockerImages().DoNotTruncate(true).ShowAll(true).Execute();`

Return a list of all containers and do not truncate the data:

`await new DockerImages().DoNotTruncate(true).ShowAll(true).Execute();`

Check out the Wiki for full documentation and examples.