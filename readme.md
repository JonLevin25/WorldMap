# WorldMap
[![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](https://github.com/jonlevin25/WorldMap/blob/master/LICENSE)
[![openupm](https://img.shields.io/npm/v/com.jonlevin.worldmap?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.jonlevin.worldmap/)

World Map is a plugin for creating 3D level-select / progression maps in Unity

<a href="http://www.youtube.com/watch?feature=player_embedded&v=H9xcQNLpf4w" target="_blank">
Demo<br />
<img src="http://img.youtube.com/vi/H9xcQNLpf4w/0.jpg" 
alt="World Map Demo Video" width="240" height="180" border="3" /></a>

## Installing with openupm
This package is available on the [openupm registry](https://openupm.com). You can install it via [openupm-cli](https://github.com/openupm/openupm-cli).
```
openupm add com.jonlevin.worldmap
```

## Install via manifest.json (git url)
Alternatively, you can also install by manually modifying your **manifest.json**:<br />

1. Add the package to the 'dependencies' section:
```
"com.jonlevin.worldmap": "git+https://github.com/JonLevin25/WorldMap.git#upm"
```

2. add this section enable the project's dependencies (insert before or after the 'dependencies' section)
```
    "scopedRegistries": [
        {
          "name": "package.openupm.com",
          "url": "https://package.openupm.com",
          "scopes": [
            "com.dbrizov.naughtyattributes",
            "com.openupm"
          ]
        }
    ]
```

if your manifest cannot be parsed - [make sure its valid](https://jsonlint.com/), then make sure the `scopedRegistries` are within the root curly braces, but not nested in anything else.

## Versioning

[SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 


## Acknowledgments

* [dbrizov/NaughtyAttributes](https://github.com/dbrizov/NaughtyAttributes/) -  used for custom inspectors, repo used as template for github ci + parts of readme
* openUPM - free platform for hosting / distributing upm packages

