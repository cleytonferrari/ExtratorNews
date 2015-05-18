﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CsQuery;
using CsQuery.Web;

namespace ExtratorNews.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View(new UrlSeletor()
            {
                Url = "http://noticias.uol.com.br/ultimas/",
                SeletorLista = "article",
                SeletorTitulo = ".h-opacity60",
                SeletorLink = "a"
            });
        }

        [HttpPost]
        public ActionResult Index(UrlSeletor dados)
        {
            if (string.IsNullOrEmpty(dados.SeletorLista))
                dados.SeletorLista = "body";

            if (string.IsNullOrEmpty(dados.Url))
                return RedirectToAction("Index");

            var html = GetHtml(dados.Url);
            var query = CQ.Create(html);

              var linhas = query.Select(dados.SeletorLista);

              var listaNoticiaCrawler = new List<NoticiasCrawler>();

              foreach (var item in linhas)
              {
                  var temp = new NoticiasCrawler();

                  var elemento = CQ.Create(item.InnerHTML);
                  
                  temp.Titulo = elemento.Select(dados.SeletorTitulo).Text();
                  temp.Url = elemento.Select(dados.SeletorLink).Attr("href");
                  listaNoticiaCrawler.Add(temp);
              }


            ViewBag.ListaNoticias = listaNoticiaCrawler;
            return View(dados);
        }

        public ActionResult ExtrairNoticia(string url)
        {
            return View(new UrlSeletorConteudo(){Url = url});
        }

        [HttpPost]
        public ActionResult ExtrairNoticia(UrlSeletorConteudo dados)
        {
            
            if (string.IsNullOrEmpty(dados.Url))
                return RedirectToAction("Index");

            var html = GetHtml(dados.Url);
            var query = CQ.Create(html);

            var noticia = new NoticiasConteudoCrawler();

            noticia.Autor = query.Select(dados.SeletorAutor).Text();
            noticia.Titulo = query.Select(dados.SeletorTitulo).Text();
            noticia.Conteudo = query.Select(dados.SeletorConteudo).Text();
            noticia.UrlFoto = query.Select(dados.SeletorFoto).Attr("src");


            ViewBag.Noticia = noticia;
            return View(dados);
        }



        private static string GetHtml(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0";
            request.Method = "GET";
            // make request for web page
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader htmlSource = new StreamReader(response.GetResponseStream());

            string htmlStringSource = string.Empty;
            htmlStringSource = htmlSource.ReadToEnd();
            response.Close();
            return htmlStringSource;
        }
    }

    public class UrlSeletor
    {
        [Display(Name = "Url do Site:")]
        public string Url { get; set; }

        [Display(Name = "Seletor para a lista de notícias:")]
        public string SeletorLista { get; set; }

        [Display(Name = "Seletor do título da notícia:")]
        public string SeletorTitulo { get; set; }

        [Display(Name = "Seletor do link da notícia:")]
        public string SeletorLink { get; set; }
    }

    public class NoticiasCrawler
    {
        public string Titulo { get; set; }
        public string Url { get; set; }
    }

    public class UrlSeletorConteudo
    {
        [Display(Name = "Url da notícia:")]
        public string Url { get; set; }
        
        [Display(Name = "Seletor do autor:")]
        public string SeletorAutor { get; set; }

        [Display(Name = "Seletor do Título:")]
        public string SeletorTitulo { get; set; }

        [Display(Name = "Seletor do conteúdo:")]
        public string SeletorConteudo { get; set; }

        [Display(Name = "Seletor da foto:")]
        public string SeletorFoto { get; set; }
    }

    public class NoticiasConteudoCrawler
    {
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string Conteudo { get; set; }
        public string UrlFoto { get; set; }
    }


}
