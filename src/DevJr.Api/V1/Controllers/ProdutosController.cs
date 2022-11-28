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
    [Route("api/v{version:apiVersion}/Produtos")]
    [ApiController]
    public class ProdutosController : MainController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IProdutoService _produtoService;
        private readonly IMapper _mapper;

        public ProdutosController(IProdutoRepository produtoRepository, 
                                  IFornecedorRepository fornecedorRepository, 
                                  IMapper mapper,
                                  IProdutoService produtoService,
                                  INotificador notificador,
                                  IUser user) : base(notificador,user)
        {
            _produtoRepository = produtoRepository;
            _fornecedorRepository = fornecedorRepository;
            _mapper = mapper;
            _produtoService = produtoService;
        }
        [HttpGet]

        public async Task<IEnumerable<ProdutoViewModel>>ObterTodos()
        {

            return _mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores());


        }

        [HttpGet("{id}")]

        public async Task<ActionResult<ProdutoViewModel>> ObterPorId(Guid id)
        {

            var produtoViewModel = await ObterProduto(id);
            if (produtoViewModel == null) return NotFound();
            return produtoViewModel;


        }

        [ClaimsAuthorize("Produto", "Adicionar")]
        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>>Adicionar(ProdutoViewModel produtoViewModel)
        {

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imagemNome = Guid.NewGuid() + "_" + produtoViewModel.Imagem;
            if(!UploadArquivo(produtoViewModel.ImagemUpload,imagemNome))
            {

                return CustomResponse(produtoViewModel);

            }
            produtoViewModel.Imagem = imagemNome;   
            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));
            return CustomResponse(produtoViewModel);
        }



        [ClaimsAuthorize("Produto", "Adicionar")]
        [HttpPost("Adicionar")]
        public async Task<ActionResult<ProdutoViewModel>> AdicionarAlternativo(ProdutoImagemViewModel produtoViewModel)
        {

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imgPrefixo = Guid.NewGuid() + "_" ;
            if (!await UploadAlternativo(produtoViewModel.ImagemUpload, imgPrefixo))
            {

                return CustomResponse(produtoViewModel);

            }
            produtoViewModel.Imagem = imgPrefixo + produtoViewModel.ImagemUpload.FileName;
            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));
            return CustomResponse(produtoViewModel);
        }



        [RequestSizeLimit(40000000)]
        [HttpPost("Imagem")]
        public async Task<ActionResult<ProdutoViewModel>> AdicionarImagem(IFormFile file)
        {

          return Ok(file);
        }
        [ClaimsAuthorize("Produto", "Atualizar")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Atualizar(Guid id,ProdutoViewModel produtoViewModel)
        {

            if (id != produtoViewModel.Id)
            {
                NotificarErro("Os Id's informados não são iguais");
                return CustomResponse(); 

            }

            var produtoAtualizacao = await ObterProduto(id);
            produtoViewModel.Imagem = produtoAtualizacao.Imagem;
            if (!ModelState.IsValid) return CustomResponse(ModelState);
            if(produtoViewModel.ImagemUpload !=null)
            {
                var imagemNome = Guid.NewGuid() + "_" + produtoViewModel.Imagem;
                if(!UploadArquivo(produtoViewModel.ImagemUpload,imagemNome))
                {

                    return CustomResponse(ModelState);

                }

                produtoAtualizacao.Imagem = imagemNome;
            }
            produtoAtualizacao.Nome = produtoViewModel.Nome;    
            produtoAtualizacao.Descricao = produtoViewModel.Descricao;
            produtoAtualizacao.Valor = produtoViewModel.Valor;
            produtoAtualizacao.Ativo = produtoViewModel.Ativo;
            await _produtoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));
            return CustomResponse(produtoViewModel);
        }

        [ClaimsAuthorize("Produto", "Remover")]
        [HttpDelete("{id:guid}")]

        public async Task<ActionResult<ProdutoViewModel>> Excluir(Guid id)
        {

            var produto = await ObterProduto(id);
            if (produto == null) return NotFound();
            await _produtoService.Remover(id);
            return CustomResponse(produto);


        }





        private async Task<ProdutoViewModel> ObterProduto(Guid id)

        { 
          return  _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));
           
        
        }

        private async Task<bool> UploadAlternativo(IFormFile arquivo,string imgPrefixo)
        {
            if(arquivo == null || arquivo.Length == 0)
            {

              NotificarErro( "Forneça uma imagem para este produto!");
                return false;

            }
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/app/demo-webapi/src/assets", imgPrefixo + arquivo.FileName);

            if(System.IO.File.Exists(path))
            {
                NotificarErro( "Já existe um arquivo com este nome!");
                return false;

            }

            using(var stream = new FileStream(path,FileMode.Create))
            {

                await arquivo.CopyToAsync(stream);


            }

            return true;

        }



        private bool UploadArquivo(string arquivo, string imgNome)
        {
           
            if(string.IsNullOrEmpty(arquivo))
            {
                NotificarErro( "Forneça uma imagem para este produto!");
                return false;
            }



            var imageDataByteArray = Convert.FromBase64String(arquivo);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/app/demo-webapi/src/assets",imgNome);
            if(System.IO.File.Exists(filePath))
            {

                NotificarErro( "Já existe um arquivo com este nome !");
                return false;

            }

            System.IO.File.WriteAllBytes(filePath,imageDataByteArray);
            return true;

        }
    }
}
