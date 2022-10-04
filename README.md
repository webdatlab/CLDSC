# CLDSC
The "County Level Data Set Comparison" tool - a Unity3d (2021.3.6f1) data visualization of all counties in the United States. Try the Live Demo here: http://webdatlab.com/CLDSC/

![gitScreen01](https://user-images.githubusercontent.com/114690566/193705436-a2a6a5b5-e712-4adc-8116-a779c487a0d9.jpg)



USAGE: 

-Click/drag to move map around

-Mouse scrollwheel / 2-finger trackpad gesture can zoom in/out - when zoomed in enough the states will fade and counties will appear

-Click states or counties to add them to comparison list 

-Click compare to enter bar chart scene showing selected areas



DEV WORKFLOW: 

-Gather data from USDA Open Data Catalog (https://www.usda.gov/content/usda-open-data-catalog) and data.gov that have county FIPS codes included

-Use PyDataPrep/dataAggregator.py to combine county data sets into JSON format Unity3D can load using custom editor tool

-In Unity Project use "Assets/Rebuild State and County Data" that points to generated JSON files

-Build interface that makes use of / visualizes this new data 



TODO: 

-Fix county search / highlight  

-Field selection: changes the values used in the chart area

-Fix chart area panning bug

-State/county red<->yellow<->green shading based on selected field

