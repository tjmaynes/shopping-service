#!/bin/bash

set -e

check_requirements() {
    if [[ -z "$(command -v dotnet)" ]]; then
        echo "Please install 'dotnet' for running tests."
        exit 1
    fi
}

run_tests() {
    TEST_OPTIONS=""
    [[ ! -z $IS_CI ]] && TEST_OPTIONS="/p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:Exclude='[xunit*]*' --test-adapter-path:. --logger 'xunit;LogFileName=TestResults.xml' --results-directory output"

    dotnet test $TEST_OPTIONS
}

main() {
    check_requirements

    run_tests
}

main