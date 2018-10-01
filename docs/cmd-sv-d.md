# Server Definition Usage
### `d` - Define
```
Usage: sv d [options]

Options:
  -b|--baseUrl                Base Url
  -d|--device                 Device
  -rc|--requiredCapabilities  Required Capabilities
  -?|-h|--help                Show help information
``` 
## Options
- `-b|--baseUrl`  
The AIS Url with a trailing '/'.
- `-d|--device`  
The device name making the request.  Defaults to _celin_.
- `-rc|--requiredCapabilities`  
Required capabilities in subsequent requests.  See your `<AIS Url>/defaultconfig` for available capabilities.  Defaults to 'grid,processingOption'.
