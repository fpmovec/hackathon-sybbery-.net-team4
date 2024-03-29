﻿using Microsoft.AspNetCore.Mvc;
using Syberry.Web.Services.Abstractions;
using Syberry.Web.Models;
using Syberry.Web.Models.Dto;


namespace Syberry.Web.Controllers;

[ApiController]
[Route("/api")]
public class InfoController: ControllerBase
{
    private readonly IBelarusBankService _belarusBankService;
    private readonly IAlpfaBankService _alpfaBankService;
    private readonly INationalBankService _nationalBankService;

    public InfoController(
        IBelarusBankService belarusBankService,
        IAlpfaBankService alpfaBankService, INationalBankService nationalBankService)
    {
        _alpfaBankService = alpfaBankService;
        _nationalBankService = nationalBankService;
        _belarusBankService = belarusBankService;
    }
    
    [HttpGet("/banks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetBanksList()
        => Ok(Constants.BanksLists);

    [HttpGet("/banks/{bankName:bank}/currencies")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrenciesByBankName([FromRoute] string bankName)
    {
        switch (bankName.ToLower())
        {
            case "national":
                return Ok(await _nationalBankService.GetCurrencies());
            case "belarusbank":
                return Ok(_belarusBankService.GetCurrencies());
            case "alphabank":
                return Ok(await _alpfaBankService.GetCurrencies());
        }

        return BadRequest();
    }
    
    [HttpGet("/Rate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Task3 (string currencyCode, string bankName, DateTime date)
    {
        var res = new List<Bank>();
        
        var bRate = await _belarusBankService.BelarusBankRates();
        
        res.Add(bRate);
        
        var aRate = await _alpfaBankService.AlpfaBankRates();
        
        res.Add(aRate);

        var nRate = await _nationalBankService.GetNationalBankAsync();
        
        res.Add(nRate);

        var item = res.FirstOrDefault(x => x.Name == bankName);

        var a = item.Rates;
        
        var b = a.FirstOrDefault(x => x.KursDateTime == date &&  x.Name == currencyCode);

        var c = b.SellRate;
        
        return Ok(c);
    }
    
    [HttpGet("/Rate/rates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Task4 (string currencyCode, string bankName, DateTime from, DateTime to)
    {
        var res = new List<Bank>();
        
        var bRate = await _belarusBankService.BelarusBankRates();
        
        res.Add(bRate);
        
        var aRate = await _alpfaBankService.AlpfaBankRates();
        
        res.Add(aRate);

        var nRate = await _nationalBankService.GetNationalBankAsync();
        
        res.Add(nRate);
        
        var item = res.FirstOrDefault(x => x.Name == bankName);
        
        var a = item.Rates;

        var b = a.Where(x => x.KursDateTime <= to && x.KursDateTime >= from && x.Name == currencyCode).ToList();

        var c = b.Select(x => new
        {
            currency = x.SellRate,
            date = x.KursDateTime
        });
        
        return Ok(c);
    }
    
    /*[HttpGet("/Rate/rates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Task5 ([FromRoute] string currencyCode, string bankName, DateTime from, DateTime to)
    {
        var res = new List<Bank>();
        
        var bRate = await _belarusBankService.BelarusBankRates();
        
        res.Add(bRate);
        
        var aRate = await _alpfaBankService.AlpfaBankRates();
        
        res.Add(aRate);

        var list = res.Where(x => x.Name == bankName
                                  && x.Rates.Any(x => x.KursDateTime <= to
                                                      && x.KursDateTime >= from)
                                  && x.Rates.Any(x => x.Name == currencyCode));
        
        var stat = new StatisticsDto
        {
            AverageRate = list.,
            ChartImage = ,
            MaxRate = ,
            MinRate = ,
            RateAtThePeriodend = ,
            RateAtThePeriodStart = 
        }
        
        return Ok();
    }*/
}