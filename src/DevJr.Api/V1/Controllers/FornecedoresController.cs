using AutoMapper;
using DevJr.Api.Controllers;
using DevJr.Api.Extensions;
using DevJr.Api.ViewModels;
using DevJr.Business.Intefaces;
using DevJr.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevJr.Api.V1.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/fornecedores")]
    [ApiController]
    public class FornecedoresController : MainController
    {

        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IEnderecoRepository _enderecoRepository;   
        private readonly IMapper _mapper;
        private readonly IFornecedorService _fornecedorService;
    
    
        public FornecedoresController(IFornecedorRepository fornecedorRepository,
            IEnderecoRepository enderecoRepository,
            IMapper mapper
            ,IFornecedorService fornecedorService
            , INotificador notificador,
              IUser user) : base(notificador, user) 
        {
            _fornecedorRepository = fornecedorRepository; 
            _enderecoRepository = enderecoRepository;   
            _mapper = mapper; 
            _fornecedorService = fornecedorService;
           




        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FornecedorViewModel>>>ObterTodos()
        {

            var fornecedor = _mapper.Map<IEnumerable<FornecedorViewModel>> ( await  _fornecedorRepository.ObterTodos());   

         return  Ok(fornecedor);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> ObterPorId(Guid id)
        {

            var fornecedor = await _fornecedorRepository.ObterFornecedorProdutosEndereco(id);

            if (fornecedor == null ) return NotFound(); 

            return Ok(fornecedor);
        }

        [ClaimsAuthorize("Fornecedor","Adicionar")]
        [HttpPost]

        public async Task<ActionResult<FornecedorViewModel>>Adicionar(FornecedorViewModel fornecedorViewModel)
        {

            if (!ModelState.IsValid) return CustomResponse(ModelState);

         
                     
            await _fornecedorService.Adicionar(_mapper.Map<Fornecedor>(fornecedorViewModel));
          
            return CustomResponse(fornecedorViewModel);
        }



        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        [HttpPut("{id:guid}")]

        public async Task<ActionResult<FornecedorViewModel>> Atualizar(Guid id,FornecedorViewModel fornecedorViewModel)
        {
            if (id != fornecedorViewModel.Id) return BadRequest();

            if (!ModelState.IsValid) return CustomResponse(ModelState);

           await _fornecedorService.Atualizar(_mapper.Map<Fornecedor>(fornecedorViewModel));
          
            return CustomResponse(fornecedorViewModel);
        }
        [ClaimsAuthorize("Fornecedor", "Remover")]
        [HttpDelete("{id:guid}")]

        public async Task<ActionResult<FornecedorViewModel>> Excluir(Guid id)
        {
            var fornecedorViewModel = _fornecedorRepository.ObterFornecedorEndereco(id);
            
            if(fornecedorViewModel == null) return NotFound();

           await _fornecedorService.Remover(id);
         

            return CustomResponse();
        }

        [HttpGet("obter-endereco/{id:guid}")]
        public async Task<EnderecoViewModel> ObterFornecedorEnderecoPorId(Guid id)
        {
           return _mapper.Map<EnderecoViewModel>(await _enderecoRepository.ObterPorId(id));


        }
        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        [HttpPut("atualizar-endereco/{id:guid}")]
        public async Task<ActionResult> AtualizarEndereco(Guid id,EnderecoViewModel enderecoViewModel)
        {
            if (id != enderecoViewModel.Id) return BadRequest();

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _fornecedorService.AtualizarEndereco(_mapper.Map<Endereco>(enderecoViewModel));

          return CustomResponse(enderecoViewModel);


        }


    }
}
