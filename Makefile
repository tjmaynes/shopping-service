ENVIRONMENT := development

include .env.$(ENVIRONMENT)
export $(shell sed 's/=.*//' .env.$(ENVIRONMENT))

install_dependencies:
	./scripts/$@.sh

test:
	./scripts/$@.sh

build:
	dotnet $@

start:
	docker-compose up

build_artifact:
	dotnet publish -c Release -o dist

archive_artifact: build_artifact
	zip -r ShoppingCartAPI.zip dist

build_image:
	./scripts/$@.sh

push_image:
	./scripts/$@.sh

deploy_image: build_image push_image

deploy: archive_artifact deploy_image

ship_it: build test deploy

create_db_init_script:
	./scripts/$@.sh

run_local_db: create_db_init_script
	docker-compose run --service-ports $(DB_NAME)-db
	docker-compose down

debug_local_db:
	docker-compose run \
	$(DB_NAME)-db psql \
	-h $(DB_HOST) \
	-U $(DB_USER) \
	$(DB_NAME)