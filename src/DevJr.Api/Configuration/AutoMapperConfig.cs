﻿using AutoMapper;
using DevJr.Api.ViewModels;
using DevJr.Business.Models;

namespace DevJr.Api.Configuration
{
    public class AutoMapperConfig:Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Fornecedor, FornecedorViewModel>().ReverseMap();
            CreateMap<Endereco, EnderecoViewModel>().ReverseMap();
            CreateMap< ProdutoViewModel, Produto>();
            CreateMap<ProdutoImagemViewModel,Produto>().ReverseMap();
            CreateMap<Produto, ProdutoViewModel>().ForMember(dest => dest.NomeFornecedor, opt => opt.MapFrom(src => src.Fornecedor.Nome));
        }


    }
}
