#!/bin/bash -eu

############################################################################################################################################
#- Purpose: The purpose of this bash script is to create a resource group, blob storage account and a blog storage container called 'demo'
#- Parameters are:
#- [-r] Azure resource group name
#- [-l] Azure location (ex: eastus, westus)
#- [-a] Azure storage account name
############################################################################################################################################

# Loop, get parameters & remove any spaces from input
while getopts "r:l:a:" opt; do
    case $opt in
        r)
            # Resurce Group Name
            resourceGroupName=$OPTARG
        ;;
        l)
            # Azure Location
            location=$OPTARG
        ;;
        a)
            # Storage Account Name
            storageAccountName=$OPTARG
        ;;
        \?)            
            # If user did not provide required parameters then show usage.
            echo "Invalid parameters! Required parameters are:  [-r] resourceGroupName [-l] location [-a] storageAccountName"
        ;;   
    esac
done

# If user did not provide required parameters then non-usage.
if [[ $# -eq 0 || -z $resourceGroupName || -z $location || -z $storageAccountName ]]; then
    echo "Invalid parameters! Required parameters are:  [-r] resourceGroupName [-l] location [-a] storageAccountName"
    exit 1; 
fi

# Create resource group
az group create -n $resourceGroupName -l $location

# Check to ensure resource group was created
rgExists=$(az group exists -n $resourceGroupName --output tsv)

if [[ $rgExists == "false"  ]]; then
    echo "The script was not able to create Resource Group: $resourceGroupName."
    exit 1; 
fi

# Create storage account
az storage account create -n $storageAccountName -g $resourceGroupName -l $location --sku Standard_LRS --kind BlobStorage --access-tier Cool

# Check to ensure storage accounted was created
saDeployedName=$(az storage account list -g $resourceGroupName --query '[].name'  -o tsv)

if [[ $saDeployedName != $storageAccountName  ]]; then
    echo "The script was not able to create Storage Account: $resourceGroupName."
    exit 1; 
fi

# Create the container
storageAccountKey1=$(az storage account keys list -n $storageAccountName -g $resourceGroupName --query '[0].value' -o tsv)
az storage container create -n "demo" --account-name $storageAccountName --account-key $storageAccountKey1

# Notify the user that azure resources have been created
echo "The script has created the Azure Resources"