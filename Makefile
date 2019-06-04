IMAGE_NAME := shopping-service
SHOPPING_SERVICE_ENVIRONMENT := development
SHOPPING_SERVICE_DB_CONNECTION_STRING := mongodb://localhost:27017
REGISTRY_USERNAME := tjmaynes
REGISTRY_PASSWORD := ""
TAG := 0.1.0

build_image: guard-REGISTRY_USERNAME guard-TAG
	docker build --no-cache -t $(REGISTRY_USERNAME)/$(IMAGE_NAME):$(TAG) .

run_image:
	(docker rm -f $(IMAGE_NAME) || true) && docker run -d \
       --name $(IMAGE_NAME) \
	   --net $(IMAGE_NAME)-network \
	   --env SHOPPING_SERVICE_DB_CONNECTION_STRING=$(SHOPPING_SERVICE_DB_CONNECTION_STRING) \
	   --env SHOPPING_SERVICE_ENVIRONMENT=$(SHOPPING_SERVICE_ENVIRONMENT) \
	   --publish 3000:80 \
	   $(REGISTRY_USERNAME)/$(IMAGE_NAME):$(TAG)

push_image: guard-REGISTRY_USERNAME guard-REGISTRY_PASSWORD guard-TAG
	docker login -u --username "$(REGISTRY_USERNAME)" --password "$(REGISTRY_PASSWORD)"
	docker push $(REGISTRY_USERNAME)/$(IMAGE_NAME):$(TAG) 

install_dependencies:
	dotnet restore

setup_docker_network:
	(docker network rm $(IMAGE_NAME)-network || true) && docker network create $(IMAGE_NAME)-network

start_local_db: setup_docker_network
	(docker rm -f $(IMAGE_NAME)-db || true) && docker run -d \
		--name $(IMAGE_NAME)-db \
		--net $(IMAGE_NAME)-network \
		-p 27017:27017 \
		mongo:4.1

local_test:
	SHOPPING_SERVICE_DB_CONNECTION_STRING=$(SHOPPING_SERVICE_DB_CONNECTION_STRING) \
	SHOPPING_SERVICE_ENVIRONMENT=development \
	dotnet test

test:
	SHOPPING_SERVICE_DB_CONNECTION_STRING=$(SHOPPING_SERVICE_DB_CONNECTION_STRING) \
	SHOPPING_SERVICE_ENVIRONMENT=$(SHOPPING_SERVICE_ENVIRONMENT) \
	dotnet test \
	/p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:Exclude="[xunit*]*" \
	--test-adapter-path:. --logger "xunit;LogFileName=TestResults.xml" --results-directory output

run_service:
	dotnet run --project ShoppingService.Api

build_artifact:
	dotnet publish -c Release -o dist

archive_artifact: build_artifact
	cd ShoppingService.Api && zip -r ../ShoppingServiceArtifact.zip dist

extract_artifact:
	unzip ShoppingServiceArtifact.zip

guard-%:
	@ if [ "${${*}}" = "" ]; then \
		echo "Environment variable $* not set!"; \
		exit 1; \
	fi
