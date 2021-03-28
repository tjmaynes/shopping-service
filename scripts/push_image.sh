#!/bin/bash

set -e

check_requirements() {
    if [[ -z "$(command -v docker)" ]]; then
        echo "Please install 'docker' for building a docker image."
        exit 1
    elif [[ -z "$REGISTRY_USERNAME" ]]; then
        echo "Please provide a 'REGISTRY_USERNAME' for building a docker image"
        exit 1
    elif [[ -z "$DOCKER_IMAGE_NAME" ]]; then
        echo "Please provide a 'DOCKER_IMAGE_NAME' for building a docker image"
        exit 1
    elif [[ -z "$DOCKER_IMAGE_TAG" ]]; then
        echo "Please provide a 'DOCKER_IMAGE_TAG' for building a docker image"
        exit 1
    fi
}

push_image() {
	echo "$REGISTRY_PASSWORD" | docker login -u --username "$REGISTRY_USERNAME" --password-stdin
	docker push $REGISTRY_USERNAME/$DOCKER_IMAGE_NAME:$DOCKER_IMAGE_TAG
}

main() {
    check_requirements

    push_image
}

main
