#!/bin/bash -eu

############################################################################################################################################
#- Purpose: The purpose of this bash script is to delete a resource group.
#- Parameters are:
#- [-r] Azure resource group name
############################################################################################################################################

# Loop, get parameters & remove any spaces from input
while getopts "r:" opt; do
    case $opt in
        r)
            # Resurce Group Name
            resourceGroupName=$OPTARG
        ;;
        \?)            
            # If user did not provide required parameters then show usage.
            echo "Invalid parameters! Required parameters are:  [-r] resourceGroupName"
        ;;   
    esac
done

# If user did not provide required parameters then non-usage.
if [[ $# -eq 0 || -z $resourceGroupName ]]; then
    echo "Invalid parameters! Required parameters are:  [-r] resourceGroupName"
    exit 1; 
fi

# Create resource group
az group delete -n $resourceGroupName

# Check to ensure resource group was created
rgExists=$(az group exists -n $resourceGroupName --output tsv)

if [[ $rgExists == "true"  ]]; then
    echo "The script was not able to delete Resource Group: $resourceGroupName."
    exit 1; 
fi

# Notify the user that azure resources have been deleted
echo "The script has deleted the Azure Resources"