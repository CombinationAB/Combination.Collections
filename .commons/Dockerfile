FROM ghcr.io/sports-global/commons:base-v1 AS base
COPY --link=true --from=ghcr.io/sports-global/commons-compilers-dotnet:8 / /
COPY --link=true --from=ghcr.io/sports-global/commons-deploy:v1 /usr/local /usr/local
COPY --link=true --from=ghcr.io/sports-global/cdeploy:v3 / /
COPY --link=true --from=ghcr.io/sports-global/cdeploy-templates:v1 / /

FROM base AS sonar
COPY --link=true --from=ghcr.io/sports-global/commons-compilers-java:22 /usr/local /usr/local
COPY --link=true --from=ghcr.io/sports-global/commons-testing-sonar /usr/local /usr/local
FROM base
