using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DevIO.App.ViewModels;
using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using DevIO.App.Extensions;

namespace DevIO.App.Controllers
{
    [Authorize]
    public class FornecedoresController : BaseController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IFornecedorService _fornecedorService;
        private readonly IMapper _mapper;

        public FornecedoresController(IFornecedorRepository fornecedorRepository,
                                      IFornecedorService fornecedorService,
                                      IMapper mapper,
                                      INotificador notificador) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            _fornecedorService = fornecedorService;
            _mapper = mapper;
        }

        // GET: Fornecedores
        [AllowAnonymous]
        [Route("lista-de-forcenedores")]
        public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos()));
        }

        // GET: Fornecedores/Details/5
        [AllowAnonymous]
        [Route("dados-do-fornecedor/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var fornecedorViewModel = await ObterFornecedorEndereco(id);

            if (fornecedorViewModel == null)
            {
                return NotFound();
            }

            return View(fornecedorViewModel);
        }

        // GET: Fornecedores/Create
        [ClaimsAuthorize("Fornecedor", "Adicionar")]
        [Route("novo-fornecedor")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fornecedores/Create
        [ClaimsAuthorize("Fornecedor", "Adicionar")]
        [Route("novo-fornecedor")]
        [HttpPost]
        public async Task<IActionResult> Create(FornecedorViewModel fornecedorViewModel)
        {
            if (!ModelState.IsValid) return View(fornecedorViewModel);

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);

            await _fornecedorService.Adicionar(fornecedor);

            if (!OperacaoValida()) return View(fornecedorViewModel);

            TempData["Sucesso"] = "Fornecedor cadastrado com sucesso!";

            return RedirectToAction("Index");
        }

        // GET: Fornecedores/Edit/5
        [ClaimsAuthorize("Fornecedor", "Editar")]
        [Route("editar-fornecedor/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var fornecedorViewModel = await ObterFornecedorProdutosEndereco(id);

            if (fornecedorViewModel == null)
            {
                return NotFound();
            }

            return View(fornecedorViewModel);
        }

        // POST: Fornecedores/Edit/5
        [ClaimsAuthorize("Fornecedor", "Editar")]
        [Route("editar-fornecedor/{id:guid}")]
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, FornecedorViewModel fornecedorViewModel)
        {
            if (id != fornecedorViewModel.Id) return NotFound();

            if (!ModelState.IsValid) return View(fornecedorViewModel);

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);

            await _fornecedorService.Atualizar(fornecedor);

            if (!OperacaoValida()) return View(await ObterFornecedorProdutosEndereco(id));

            TempData["Sucesso"] = "Fornecedor atualizado com sucesso!";

            return RedirectToAction("Index");
            
        }

        // GET: Fornecedores/Delete/5
        [ClaimsAuthorize("Fornecedor", "Excluir")]
        [Route("excluir-fornecedor/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var fornecedorViewModel = await ObterFornecedorEndereco(id);

            if (fornecedorViewModel == null)
            {
                return NotFound();
            }

            return View(fornecedorViewModel);
        }

        // POST: Fornecedores/Delete/5
        [ClaimsAuthorize("Fornecedor", "Excluir")]
        [Route("excluir-fornecedor/{id:guid}")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var fornecedor = await ObterFornecedorEndereco(id);

            if (fornecedor == null)
            {
                return NotFound();
            }

            await _fornecedorService.Remover(id);

            if (!OperacaoValida()) return View(fornecedor);

            TempData["Sucesso"] = "Fornecedor excluido com sucesso!";

            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        [Route("obter-endereco-fornecedor/{id:guid}")]
        public async Task<IActionResult> ObterEndereco(Guid id)
        {
            var fornecedor = await ObterFornecedorEndereco(id);

            if (fornecedor == null)
            {
                return NotFound();
            }

            return PartialView("_DetalhesEndereco", fornecedor);
        }

        [ClaimsAuthorize("Fornecedor", "Editar")]
        [Route("atualizar-endereco-fornecedor/{id:guid}")]
        public async Task<IActionResult> AtualizarEndereco(Guid id)
        {
            var fornecedor = await ObterFornecedorEndereco(id);

            if (fornecedor == null)
            {
                return NotFound();
            }

            return PartialView("_AtualizarEndereco", new FornecedorViewModel { Endereco = fornecedor.Endereco });
        }

        [ClaimsAuthorize("Fornecedor", "Editar")]
        [Route("atualizar-endereco-fornecedor/{id:guid}")]
        [HttpPost]
        public async Task<IActionResult> AtualizarEndereco(FornecedorViewModel fornecedorViewModel)
        {
            ModelState.Remove("Nome");
            ModelState.Remove("Documento");

            if (!ModelState.IsValid) return PartialView("_AtualizarEndereco", fornecedorViewModel);

            await _fornecedorService.AtualizarEndereco(_mapper.Map<Endereco>(fornecedorViewModel.Endereco));

            if (!OperacaoValida()) return PartialView("_AtualizarEndereco", fornecedorViewModel);

            TempData["Sucesso"] = "Endereço atualizado com sucesso!";

            var url = Url.Action("ObterEndereco", "Fornecedores", new { id = fornecedorViewModel.Endereco.FornecedorId });

            return Json(new { success = true, url });
        }

        private async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(id));
        }

        private async Task<FornecedorViewModel> ObterFornecedorProdutosEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
        }
    }
}
