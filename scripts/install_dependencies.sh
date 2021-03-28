#!/bin/bash

set -e

check_requirements() {
    if [[ -z "$(command -v dotnet)" ]]; then
        echo "Please install 'dotnet' for running tests."
        exit 1
    fi
}

main() {
   check_requirements
   
   dotnet restore
}

main