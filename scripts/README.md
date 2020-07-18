# Overview

The scripts are used to create an Azure Storage Account & a clean-up script to delete the Azure Storage Account.

## create-az-storage-account Bash Script

Bash script used to create an Azure Blob Storage Account and a container called *demo* so a user can create a new Azure EventGrid Blob Created Subscription and upload a file.

To run the script, execute the following command:

```bash
    chmod +x create-az-storage-account.sh &&
    ./create-az-storage-account.sh -r {name_of_resource_group} -l {location} -a {name_of_blob_storage_account}
```

## delete-az-storage-account Bash Script

Bash script used to delete the Azure Resource Group and Azure Blob Storage Account.

To run the script, execute the following command:

```bash
    chmod +x delete-az-storage-account.sh &&
    ./delete-az-storage-account.sh -r {name_of_resource_group}
```
