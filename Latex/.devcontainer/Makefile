# Use buildx by default.
DOCKER_BUILDKIT := 1
export DOCKER_BUILDKIT

BASE_IMAGE=$(shell grep '^FROM' Dockerfile | awk '{print $$2}')

all: build-container-image

build-container-image:
	docker pull $(BASE_IMAGE)
	docker build -t latex-project-container .
