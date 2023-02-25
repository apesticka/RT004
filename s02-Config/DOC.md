# Config documentation
The config file expects an xml format  
There has to be a root **\<config>** node

## Possible top level nodes are:  
* **\<width>** - the width of the HDR image
* **\<height>** - the height of the HDR image
* **\<output>** - the name of the output file
* **\<background>** - the background color
    * expects a **\<color>** node

## Nodes for additional parameters:
* **\<color>** - expects 3 child nodes
    * **\<r>**
    * **\<g>**
    * **\<b>**
    * all child nodes currently expect float values from 0 to 1

(most of this is temporary and it's going to change soon)