#!/usr/bin/python
# -Start with loading base state/county JSON
# -then load/loop through each data set:
#   -if state/county match, merge into JSON structure with data set ID as key
# -output merged JSON for unity to use

"""
TODO:
-tqdm for progress, each data set is a step
-color coded output
"""


import pandas as pd
import os, sys, pprint, json, traceback

stateCountyMergedData = {"state":{}, "county":{}}
stateAbbreviations = []

def getStateNameOrAbbreviation(lookup):
    for pair in stateAbbreviations:
        if(lookup.lower() == pair[0].lower()):
            return pair[1]
        elif(lookup.lower() == pair[1].lower()):
            return pair[0]

def pprintAndQuit(structure):
    pprint.pprint(structure)
    os._exit(0)

def cleanNumericField(numVal):
    return int(numVal.replace("\"", ""))
def cleanFloatField(numVal):
    return round(float(numVal.replace("\"", "")), 2)

with open("./StateCountyGeoJSON/us_features.json") as stateFile:
    print("* Processing us_features.json")
    usStates = json.load(stateFile)

for abset in stateAbbreviations:
    print("meshInfo = JsonConvert.DeserializeObject<MeshInfo>(File.ReadAllText(@\"PyDataPrep/StateJSON/"+abset[1]+".unity.json\"));\n")
    print("CreateShapeWithTriangles(\""+abset[0]+"\", meshInfo.vertices, meshInfo.triangles);\n")

with open("./StateCountyGeoJSON/us-counties.json") as countiesFile:
    print("* Processing us-counties.json")
    usCounties = json.load(countiesFile)
    for feature in usCounties["features"]:
        try:
            #stateCountyMergedData["county"][feature["id"]]["geometry"] = feature["geometry"]["coordinates"]
            stateCountyMergedData["county"][int(feature["id"])] = {}
        except:
            traceback.print_exc()

with open("./StateCountyGeoJSON/countyFIPS.tsv") as countiesFile:
    print("* Processing countyFIPS.tsv")
    fipsLines = countiesFile.readlines()
    for line in fipsLines:
        fipsData = line.replace('\n','').replace("\\\"", "").split("\t")
        #pprint.pprint(fipsData)
        newCounty = {"name": fipsData[1], "state": fipsData[2]} #, "fips": fipsData[0]}
        stateCountyMergedData["county"][int(fipsData[0])] = newCounty

# --------------------- COUNTY LEVEL DATA ------------------------

# Natural Amenities [COMPLETE] ---------------------------------------------------------------------------------------------------
with open("./ERS/USDANatualAmenities/natamenf_dataonly.csv") as countiesFile:
    print("* Processing ERS/USDANatualAmenities/natamenf_dataonly.csv")
    """ cols: 
    0 fips
    1 fips2 
    2 stateabbr 
    3 countyname 
    4 census 
    5 ruralurbancontinuum (needs lookup table)
    6 urban influence code (needs lookup table)
    11 Topography code (needs lookup table)
    12 percent water area
    """
    csvLines = countiesFile.readlines()
    #for county in stateCountyMergedData["county"]:
    for line in csvLines:
        rowData = line.replace("\n", "").replace("\\\"", "").split(",")
        fips = int(rowData[0])
        try:
            if(fips in stateCountyMergedData["county"]):
                stateCountyMergedData["county"][fips]["censusArea"] = cleanNumericField(rowData[4])
                stateCountyMergedData["county"][fips]["ruralUrbanContinuum"] = cleanNumericField(rowData[5])
                stateCountyMergedData["county"][fips]["urbanInfluenceCode"] = cleanNumericField(rowData[6])
                stateCountyMergedData["county"][fips]["meanTempJan"] = cleanFloatField(rowData[7])
                stateCountyMergedData["county"][fips]["meanSunlightJan"] = cleanNumericField(rowData[8])
                stateCountyMergedData["county"][fips]["meanTempJul"] = cleanFloatField(rowData[9])
                stateCountyMergedData["county"][fips]["meanHumidityJul"] = cleanNumericField(rowData[10])
                stateCountyMergedData["county"][fips]["topographyCode"] = cleanNumericField(rowData[11])
                stateCountyMergedData["county"][fips]["percentWaterArea"] = cleanFloatField(rowData[12])
        except:
            traceback.print_exc()

# Poverty [COMPLETE] ---------------------------------------------------------------------------------------------------
with open("./ERS/USDACountyLevelDataSets/PovertyEstimates_dataOnly.csv") as countiesFile:
    print("* Processing ERS/USDACountyLevelDataSets/PovertyEstimates_dataOnly.csv")
    """ cols: 
    0 fips
    1 stateabbr 
    2  stname
    7 population in poverty 
    10 % in poverty
    25 median income
    """
    csvLines = countiesFile.readlines()
    #for county in stateCountyMergedData["county"]:
    for line in csvLines:
        rowData = line.replace("\n", "").split(",")

        fips = int(rowData[0])
        try:
            if(fips in stateCountyMergedData["county"]):
                if (float(rowData[25].replace("\"", "")) < 20):
                    pprintAndQuit(rowData)
                stateCountyMergedData["county"][fips]["povertyPercent"] = cleanFloatField(rowData[10])
                stateCountyMergedData["county"][fips]["childPovertyPercent"] = cleanFloatField(rowData[16])
                stateCountyMergedData["county"][fips]["medianIncome"] = cleanFloatField(rowData[25])
        except:
            traceback.print_exc()

# Rural Atlas [COMPLETE] ---------------------------------------------------------------------------------------------------
df = pd.read_excel('./ERS/RuralAtlasData23.xlsx', sheet_name='People')
print("* Processing ERS/RuralAtlasData23.xlsx")
for i, row in df.iterrows():
    try:
        fips = int(row['FIPS'])
        if(fips in stateCountyMergedData["county"]):
            stateCountyMergedData["county"][fips]["immigrantPercent"] = round(float(row['ForeignBornPct']), 2)
            stateCountyMergedData["county"][fips]["ownHomePercent"] = round(float(row['OwnHomePct']), 2)
        else:
            pass #print("FIPS ["+str(row["FIPS"])+"] NOT FOUND")
    except:
        traceback.print_exc()

df = pd.read_excel('./ERS/RuralAtlasData23.xlsx', sheet_name='Jobs')
for i, row in df.iterrows():
    try:
        fips = int(row['FIPS'])
        if(fips in stateCountyMergedData["county"]):
            stateCountyMergedData["county"][fips]["unemploymentPercent"] = round(float(row['UnempRate2020']), 2)
            stateCountyMergedData["county"][fips]["agricultureEmploymentPercent"] = round(float(row['PctEmpAgriculture']), 2)
            stateCountyMergedData["county"][fips]["miningEmploymentPercent"] = round(float(row['PctEmpMining']), 2)
            stateCountyMergedData["county"][fips]["constructionEmploymentPercent"] = round(float(row['PctEmpConstruction']), 2)
            stateCountyMergedData["county"][fips]["manufacturingEmploymentPercent"] = round(float(row['PctEmpManufacturing']), 2)
            stateCountyMergedData["county"][fips]["tradeEmploymentPercent"] = round(float(row['PctEmpTrade']), 2)
            stateCountyMergedData["county"][fips]["transportationEmploymentPercent"] = round(float(row['PctEmpTrans']), 2)
            stateCountyMergedData["county"][fips]["informationEmploymentPercent"] = round(float(row['PctEmpInformation']), 2)
            stateCountyMergedData["county"][fips]["fireEmploymentPercent"] = round(float(row['PctEmpFIRE']), 2)
            stateCountyMergedData["county"][fips]["serviceEmploymentPercent"] = round(float(row['PctEmpServices']), 2)
            stateCountyMergedData["county"][fips]["governmentEmploymentPercent"] = round(float(row['PctEmpGovt']), 2)
        else:
            pass #print("FIPS ["+str(row["FIPS"])+"] NOT FOUND")
    except:
        traceback.print_exc()

""" -------------- ERS Poverty data from 2020 is newer ------------
df = pd.read_excel('./ERS/RuralAtlasData23.xlsx', sheet_name='Income')
for i, row in df.iterrows():
    try:
        fips = int(row['FIPS'])
        if(fips in stateCountyMergedData["county"]):
            stateCountyMergedData["county"][fips]["childPovertyPercent"] = round(float(row['Poverty_Rate_0_17_ACS']), 2)
        else:
            pass #print("FIPS ["+str(row["FIPS"])+"] NOT FOUND")
    except:
        traceback.print_exc()
"""

df = pd.read_excel('./ERS/RuralAtlasData23.xlsx', sheet_name='Veterans')
for i, row in df.iterrows():
    try:
        fips = int(row['FIPS'])
        if(fips in stateCountyMergedData["county"]):
            stateCountyMergedData["county"][fips]["veteransInPovertyPercent"] = round(float(row['PctVetsPoor']), 2)
            stateCountyMergedData["county"][fips]["veteransUnemploymentPercent"] = round(float(row['UEVetsRate']), 2)
        else:
            pass #print("FIPS ["+str(row["FIPS"])+"] NOT FOUND")
    except:
        traceback.print_exc()


# Education [COMPLETE] ---------------------------------------------------------------------------------------------------
with open("./ERS/USDACountyLevelDataSets/Education_dataOnly.csv") as countiesFile:
    print("* Processing ERS/USDACountyLevelDataSets/Education_dataOnly.csv")
    """ cols: 
    0 fips
    51 noHSDiploma 
    52 onlyHSDiploma
    53 someCollege 
    54 bachelorsDegree
    """
    csvLines = countiesFile.readlines()
    for line in csvLines:
        rowData = line.replace("\n", "").replace("\\\"", "").split(",")
        fips = int(rowData[0])
        if(rowData[-1] == ''):
            continue


        if(float(rowData[-1]) > 100.0):
            pprintAndQuit(rowData)

        try:
            if(fips in stateCountyMergedData["county"]):
                stateCountyMergedData["county"][fips]["noHSDiplomaPercent"] = cleanFloatField(rowData[-4])
                stateCountyMergedData["county"][fips]["onlyHSDiplomaPercent"] = cleanFloatField(rowData[-3])
                stateCountyMergedData["county"][fips]["someCollegePercent"] = cleanFloatField(rowData[-2])
                stateCountyMergedData["county"][fips]["bachelorsDegreePercent"] = cleanFloatField(rowData[-1])
        except:
            traceback.print_exc()

# Oil / Gas Production [COMPLETE] ---------------------------------------------------------------------------------------------------
def oilGasChangeCode(codeName):
    if(codeName.lower() == "status quo"):
        return 0
    if(codeName.lower() == "h_growth"):
        return 1
    if(codeName.lower() == "h_decline"):
        return -1
    return 0

df = pd.read_excel('./ERS/USDAOilGasProduction/oilgascounty.xls', sheet_name='oilgascounty')
print("* Processing ERS/USDAOilGasProduction/oilgascounty.xls")
for i, row in df.iterrows():
    try:
        fips = int(row['geoid']) # Why geoid? FIPS is same value without 0 padding...
        if(fips in stateCountyMergedData["county"]):
            stateCountyMergedData["county"][fips]["oilBarrelProduction2011"] = int(str(row['oil2011']).replace(",", ""))
            stateCountyMergedData["county"][fips]["gas1kCubicFeetProduction2011"] = int(str(row['gas2011']).replace(",", ""))
            stateCountyMergedData["county"][fips]["oilChange"] = oilGasChangeCode(row['oil_change_group'])
            stateCountyMergedData["county"][fips]["gasChange"] = oilGasChangeCode(row['gas_change_group'])
        else:
            pass #print("FIPS ["+str(row["FIPS"])+"] NOT FOUND")
    except:
        traceback.print_exc()


# Environmental Quality Index [COMPLETE] ---------------------------------------------------------------------------
df = pd.read_csv('./EPA/EnvironmentalQualityIndex/PCA_Input_Variables_Non_Transformed.csv')
print("* Processing EPA/EnvironmentalQualityIndex/PCA_Input_Variables_Non_Transformed.csv")
airPollutantTonsFields = ['a_edb', 'a_formaldehyde', 'a_teca', 'a_112tca', 'a_dbcp', 'a_dcl_propene',
                      'a_acrylic_acid', 'a_benzidine', 'a_benzyl_cl', 'a_be', 'a_dehp', 'a_ccl4',
                      'a_cyls', 'a_cl', 'a_c6h5cl', 'a_chloroform', 'a_chloroprene', 'a_cr',
                      'a_co', 'a_cn', 'a_dbp', 'a_etcl', 'a_ebenzine', 'a_edc',
                      'a_glycol_ethers', 'a_n2h2', 'a_hcl', 'a_isophorone', 'a_mn', 'a_mebr',
                      'a_mecl2', 'a_ph3', 'a_pcbs', 'a_procl2', 'a_quinolin', 'a_c2hcl3',
                      'a_vycl'] # Tons/yr
airPollutantPPMFields = ['o3','so2','nox','co'] # parts per million
waterPollutantMGLFields = ['W_As', 'W_Ba', 'W_Cd', 'W_Cr', 'W_CN', 'W_FL', 'W_HG',
                           'W_NO3', 'W_NO2', 'W_SE', 'W_SB'] # milligrams per liter

for i, row in df.iterrows():
    try:
        fips = int(row['stfips'])
        airPollutantTons = 0.0
        airPollutantPPM = 0.0
        waterPollutantMGL = 0.0

        for field in range(0, len(airPollutantTonsFields)):
            airPollutantTons += float(row[airPollutantTonsFields[field]])
        for field in range(0, len(airPollutantPPMFields)):
            airPollutantPPM += float(row[airPollutantPPMFields[field]])
        for field in range(0, len(waterPollutantMGLFields)):
            waterPollutantMGL += float(row[waterPollutantMGLFields[field]])


        if(fips in stateCountyMergedData["county"]):
            stateCountyMergedData["county"][fips]["airPollutantTons"] = round(airPollutantTons, 2)
            stateCountyMergedData["county"][fips]["airPollutantPPM"] = round(airPollutantPPM, 2)
            stateCountyMergedData["county"][fips]["waterPollutantMGL"] = round(waterPollutantMGL, 2)
            stateCountyMergedData["county"][fips]["irrigatedAcresPercent"] = round(float(row['PCT_IRRIGATED_ACRES']), 2)
            stateCountyMergedData["county"][fips]["chemicalNematodeControlAcresPercent"] = round(float(row['pct_nematode_acres']), 2)
            stateCountyMergedData["county"][fips]["manureAcresPercent"] = round(float(row['pct_manure_acres']), 2)
            stateCountyMergedData["county"][fips]["chemicalDiseaseControlAcresPercent"] = round(float(row['pct_disease_acres']), 2)
            stateCountyMergedData["county"][fips]["chemicallyDefoliatedAcresPercent"] = round(float(row['pct_defoliate_acres']), 2)
            stateCountyMergedData["county"][fips]["harvestedAcresPercent"] = round(float(row['pct_harvested_acres']), 2)
            stateCountyMergedData["county"][fips]["animalUnitsPerAcrePercent"] = round(float(row['pct_au']), 2)
            stateCountyMergedData["county"][fips]["extremeDroughtPercent"] = round(float(row['AvgOfD3_ave']), 2)
            stateCountyMergedData["county"][fips]["publicTransitPercent"] = round(float(row['PubTrans']), 2)
            stateCountyMergedData["county"][fips]["carFatalitiesPer100k"] = round(float(row['fatalities']), 2)
            stateCountyMergedData["county"][fips]["averageCommuteTimeMinutes"] = round(float(row['CommuteTime']), 2)
        else:
            pass #print("FIPS ["+str(row["FIPS"])+"] NOT FOUND")
    except:
        traceback.print_exc()


# CDC Places [COMPLETE] (health/wellness data) ----------------------------------------------------------------------
df = pd.read_csv('./CDC/PLACES/PLACES__County_Data__GIS_Friendly_Format___2021_release.csv')
print("* Processing CDC/PLACES/PLACES__County_Data__GIS_Friendly_Format___2021_release.csv")

"""for col_name in df.columns:
    print(col_name)
sys.exit(0)"""

for i, row in df.iterrows():
    try:
        fips = int(row['CountyFIPS'])

        if(fips in stateCountyMergedData["county"]):
            stateCountyMergedData["county"][fips]["bingeDrinkingPercent"] = round(float(row['BINGE_CrudePrev']), 2)
            stateCountyMergedData["county"][fips]["noHealthInsurancePercent"] = round(float(row['ACCESS2_CrudePrev']), 2)
            stateCountyMergedData["county"][fips]["cancerDiagnosisPercent"] = round(float(row['CANCER_CrudePrev']), 2)
            stateCountyMergedData["county"][fips]["athsmaDiagnosisPercent"] = round(float(row['CASTHMA_CrudePrev']), 2)
            stateCountyMergedData["county"][fips]["depressionPercent"] = round(float(row['DEPRESSION_CrudePrev']), 2)
            stateCountyMergedData["county"][fips]["poorMentalHealthPercent"] = round(float(row['MHLTH_CrudePrev']), 2)
            stateCountyMergedData["county"][fips]["obesityPercent"] = round(float(row['OBESITY_CrudePrev']), 2)
            stateCountyMergedData["county"][fips]["strokePercent"] = round(float(row['STROKE_CrudePrev']), 2)
            stateCountyMergedData["county"][fips]["noLeisureTimePercent"] = round(float(row['LPA_CrudePrev']), 2)
            stateCountyMergedData["county"][fips]["smokerPercent"] = round(float(row['CSMOKING_CrudePrev']), 2)
        else:
            pass #print("FIPS ["+str(row["FIPS"])+"] NOT FOUND")
    except:
        traceback.print_exc()


# Federal Funding (This one has rows/columns swapped, need to pivot) ------------------------------------

# MEDSL (MIT Election Data + Science Lab)

# FSA Conservation Reserve Program



#pprint.pprint(stateCountyMergedData)
print("* Outputting merged JSON files")
with open("./MergedStateCountyData.json", 'w') as outputFile:
    json.dump(stateCountyMergedData, outputFile, sort_keys=False, indent=4, separators=(',', ': '))
with open("./MergedCountyData.json", 'w') as outputFile:
    json.dump(stateCountyMergedData["county"], outputFile, sort_keys=False, indent=4, separators=(',', ': '))
with open("./MergedStateData.json", 'w') as outputFile:
    json.dump(stateCountyMergedData["state"], outputFile, sort_keys=False, indent=4, separators=(',', ': '))

with open("./MergedCountyData_array.json", "w") as outputFile:
    jArr = []
    for key in stateCountyMergedData["county"]:
        stateCountyMergedData["county"][key]["fips"] = key
        jArr.append(stateCountyMergedData["county"][key])
    json.dump(jArr, outputFile, sort_keys=False, indent=4, separators=(',', ': '))

# Write out json file with field descriptors separately, use this for unity import metadata

