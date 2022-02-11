# .NET 6 & ElasticSearch API

This is an api which allows users to search for apartments or management companies

```c#
# Setup Elasticsearch
Setup elasticsearch(v7.10) on AWS with the free tier or setup elasticsearch in docker
AWS => https://aws.amazon.com/opensearch-service/
Docker => https://www.elastic.co/guide/en/elasticsearch/reference/current/docker.html

# Update AppSettings
Update elasticsearch BaseUrl with your baseurl from AWS or docker

# Restore dependencies
$ dotnet restore

# Upload demo file
Upload demo files in docs via the upload api endpoints and start searching... 
```