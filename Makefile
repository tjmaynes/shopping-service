IMAGE_NAME := shopping-service
SHOPPING_SERVICE_ENVIRONMENT := development
REGISTRY_USERNAME := ""
REGISTRY_PASSWORD := ""
TAG := ""

build_image: guard-REGISTRY_USERNAME guard-TAG
	docker build --no-cache -t $(REGISTRY_USERNAME)/$(IMAGE_NAME):$(TAG) .

run_image:
	(docker rm -f $(IMAGE_NAME) || true) && docker run -d \
		--name $(IMAGE_NAME) \
    	-env SHOPPING_SERVICE_ENVIRONMENT=$(SHOPPING_SERVICE_ENVIRONMENT) \
		-p 8080:80 \
		$(REGISTRY_USERNAME)/$(IMAGE_NAME):$(TAG)

push_image: guard-REGISTRY_USERNAME guard-REGISTRY_PASSWORD guard-TAG
	docker login -u --username "$(REGISTRY_USERNAME)" --password "$(REGISTRY_PASSWORD)"
	docker push $(REGISTRY_USERNAME)/$(IMAGE_NAME):$(TAG) 

install_dependencies:
	dotnet restore

start_local_db:
	(docker rm -f $(IMAGE_NAME)-db || true) && docker run -d \
		--name $(IMAGE_NAME)-db \
		-p 27017:27017 \
		mongo:4.1

local_test:
	SHOPPING_SERVICE_ENVIRONMENT=development \
	dotnet test

test:
	SHOPPING_SERVICE_ENVIRONMENT=production \
	dotnet test

run_service:
	dotnet run --project ShoppingService.Api

guard-%:
	@ if [ "${${*}}" = "" ]; then \
		echo "Environment variable $* not set!"; \
		exit 1; \
	fi

.PHONY: test
