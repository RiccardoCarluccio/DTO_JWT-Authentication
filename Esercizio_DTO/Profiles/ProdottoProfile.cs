using AutoMapper;
using System.Security.Cryptography.X509Certificates;

namespace Esercizio_DTO.Profiles
{
    public class ProdottoProfile : Profile
    {
        public ProdottoProfile()
        {
            CreateMap<Entities.Prodotto, Models.ProdottoDto>();
        }
    }
}
