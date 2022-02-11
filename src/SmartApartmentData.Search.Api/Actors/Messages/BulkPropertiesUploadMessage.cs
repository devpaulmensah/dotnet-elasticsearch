using SmartDataApartment.Search.Api.Models.ElasticSearchDTOs;

namespace SmartDataApartment.Search.Api.Actors.Messages
{
    public struct BulkPropertiesUploadMessage
    {
        public IEnumerable<PropertyUploadRequest> Properties { get; }

        public BulkPropertiesUploadMessage(IEnumerable<PropertyUploadRequest> properties)
        {
            Properties = properties;
        }
    }
}