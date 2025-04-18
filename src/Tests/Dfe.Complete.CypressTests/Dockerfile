FROM cypress/base:20.18.0@sha256:48610d161ad6dc28a6d92fbfe50d86b07c55b66290de6d910a85c049a249c031

COPY ./cypress ./cypress
COPY cypress.config.ts .
COPY package-lock.json package-lock.json
COPY package.json package.json
COPY tsconfig.json .

RUN npm install
ENTRYPOINT ["npm", "run", "cy:zap"]