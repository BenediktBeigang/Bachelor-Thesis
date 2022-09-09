FROM dockershelf/latex:full

USER root

# Be quiet!
RUN echo "set bell-style none" >> /etc/inputrc

WORKDIR /src

# Setup a container user that's probably close to the local user's UID to avoid permission issues.
ARG CONTAINER_USER=codespaces
ARG CONTAINER_UID=1000
ARG CONTAINER_GID=${CONTAINER_UID}
RUN addgroup --gid ${CONTAINER_GID} ${CONTAINER_USER} && \
    adduser --disabled-password --gecos '' --uid ${CONTAINER_UID} --gid ${CONTAINER_GID} ${CONTAINER_USER} && \
    mkdir -p /src && chown ${CONTAINER_UID}:${CONTAINER_GID} /src

# Install some extra packages.
RUN apt-get update && \
    apt-get -y --no-install-recommends install \
        vim less git \
        latex-mk latex-make \
        && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

USER ${CONTAINER_USER}
# Add some directories for vscode to bind to avoid having to rebuild extensions every launch
# and to save bash history across runs.
# https://code.visualstudio.com/docs/remote/containers-advanced#_avoiding-extension-reinstalls-on-container-rebuild
# https://code.visualstudio.com/docs/remote/containers-advanced#_persist-bash-history-between-runs
RUN mkdir -p \
    /home/${CONTAINER_USER}/.histvol \
    /home/${CONTAINER_USER}/.vscode-server/extensions \
    /home/${CONTAINER_USER}/.vscode-server-insiders/extensions && \
    touch /home/${CONTAINER_USER}/.histvol/bash_history && \
    chown -R ${CONTAINER_USER} \
        /home/${CONTAINER_USER}/.histvol \
        /home/${CONTAINER_USER}/.vscode-server \
        /home/${CONTAINER_USER}/.vscode-server-insiders && \
    echo 'shopt -s histappend && export HISTFILE="$HOME/.histvol/bash_history"' >> /home/${CONTAINER_USER}/.bashrc
