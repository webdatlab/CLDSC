import os, sys, pprint, json, time

outJSON = json.loads('{"type":"FeatureCollection","features":[]}')

# parse original geojson, split by state and run it through triangle individually
with open("us-counties.geojson.geojson") as inputFile:
    inputData = json.load(inputFile)
    for feature in inputData["features"]:
        time.sleep(0.1)
        outJSON["features"] = [feature]
        with open("input.geo.json", "w") as outFile:
            json.dump(outJSON, outFile)
            time.sleep(0.2)
        os.system("bash ./run.sh")
        os.system("mv output.unity.json '"+feature["properties"]["id"]+".unity.json'")





