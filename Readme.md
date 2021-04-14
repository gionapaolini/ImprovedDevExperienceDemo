## Table of contents
- [Getting started](#getting-started)
  - [Requirements](#requirements)
  - [Setup](#setup)
  - [Initialize the platform](#initialize-the-platform)
- [Development](#development)
  - [How to restart a service](#how-to-restart-a-service)
  - [How to debug](#how-to-debug)
    - [Debug services](#debug-services)
  - [How to create a new service](#how-to-create-a-new-service)

# Getting started

## Requirements

- [Docker](https://docs.docker.com/get-docker/)  
- [K3d](https://k3d.io/)  
- [Kubectl](https://kubernetes.io/docs/tasks/tools/#kubectl)
- [Helm](https://helm.sh/docs/intro/install/)
- [VScode](https://code.visualstudio.com/) 
- [Kubernetes extension](vscode:extension/ms-kubernetes-tools.vscode-kubernetes-tools)
  
## Setup
## Initialize the platform

1. Create the cluster
   ```
   ./Scripts/CreateCluster.sh
   ```
2. Start the development environment
 
   ```
   ./Scripts/DeploySystem.sh
   ```
   The first time it will take more time (10-15m) because it needs to download all the images and install all the module dependencies (included 2 heavy ML models), after which it will take significantly less time (unless you destroy and re-create the cluster again, in that case it will need to re-pull all the images).


# Development

This development environment is set up in such a way that the entire codebase (`Source` folder) is shared directly with the running containers within Kubernetes. This means that whatever change you do to the code, will automatically be synced. 

## How to restart a service
We use a watch tool to listen for changes to the filesystem, if a change is detected the service is automatically restarted.

With many services you might notice a decrease in performance if there are too many files to be watched. For this reason, when possible, we decide to watch only a single file, and modify that in case we want to restart the service.
For example: for NodeJS applications we use nodemon to listen to one file only. You can just add a space, save, and nodemon will automatically restart the service without killing the entire pod.  

For example by modifying `Source/NodeJS/Services/IntentEditor/app.js` you will restart the `IntentEditor` service

## How to debug

### Debug services

1. From inside vscode, press `Ctrl+Shift+D` to open the __Run and Debug__ window (Or click on the button with the play icon on the left menu)
2. Select which Service you want to attach your debugger to, and press play.
   
__You can attach to multiple service at the same time to debug multiple services.__

## How to create a new service

1. Create a new folder inside `Sources/{framework}/Services` with the name of the new service
   ```
   mkdir Sources/{framework}/Services/{NewServiceName}
   ```
   Make sure you follow the naming convention of __PascalCase__

2. Navigate to the folder `SystemCharts/ServiceChart/ServicesValues` and add a values file for the service
   1. Clone one of the other files and change the service name to the new service name (PascalCase)
      ```
      serviceName: {ServiceName}
      ```
   2. Add the environment variables that you need
      - If you need kafka: 
         ```
         - name: KAFKA_SERVER_URL
           value: "cluster-resources-kafka:9092"
         - name: KAFKA_CLIENT_ID
           value: "{servicename}"            
         - name: KAFKA_CONSUMER_GROUP
           value: "{servicename}"
         ```
      - If you need mongodb: 
         ```
         - name: MONGODB_SERVER_URL
           value: "mongodb://cluster-resources-mongodb:27017"
         ```
      And for anything else you might need just add it to the env list. 

   
&nbsp;

*This is a work in progress and more documentation will follow.*
