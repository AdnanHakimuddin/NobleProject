﻿using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using Newtonsoft.Json;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.EuropaCheckVatService;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Common
{
    public partial class CustomTask : IScheduleTask
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly INopDataProvider _nopDataProvider;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        #endregion

        #region Ctor

        public CustomTask(ILogger logger,
            INopDataProvider nopDataProvider,
            IProductService productService,
            ICategoryService categoryService)
        {
            _logger = logger;
            _nopDataProvider = nopDataProvider;
            _productService = productService;
            _categoryService = categoryService;
        }

        #endregion

        #region Utilities

        #region Methods

        public async Task<HttpResponseMessage> HttpRequestAsync<T>(string url, HttpMethod httpMethod, string token, T item = default(T))
        {
            var uri = new Uri($"{url}");

            using (var handler = new HttpClientHandler())
            {

                using (HttpClient httpClient = new HttpClient(handler))
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();

                    HttpContent httpContent = null;

                    if (token != null) { httpClient.DefaultRequestHeaders.Add("X-AUTH-TOKEN", token);/* = new AuthenticationHeaderValue("Authorization", "Bearer " + token);*/ }

                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var stringfiedJson = "";
                    if (item != null)
                    {
                        stringfiedJson = JsonConvert.SerializeObject(item);
                        await _logger.InsertLogAsync(Core.Domain.Logging.LogLevel.Debug, "Json Log", stringfiedJson);
                    }
                    httpContent = new StringContent(stringfiedJson, Encoding.UTF8, "application/json");
                    try
                    {
                        HttpResponseMessage response = null;
                        switch (httpMethod)
                        {
                            case HttpMethod.Get:
                                response = await httpClient.GetAsync(uri);
                                break;
                            case HttpMethod.Post:
                                response = await httpClient.PostAsync(uri, httpContent);
                                break;
                            case HttpMethod.Put:
                                response = await httpClient.PutAsync(uri, httpContent);
                                break;
                            case HttpMethod.Delete:
                                response = await httpClient.DeleteAsync(uri);
                                break;
                        }

                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception($@"Http request failure {response.StatusCode} {uri} {httpMethod.ToString()}");
                        }
                        return response;
                    }
                    catch (Exception ex)
                    {
                        HttpResponseMessage response = null;
                        await _logger.InsertLogAsync(Core.Domain.Logging.LogLevel.Debug, "Exception Error :: ", ex.Message);
                        return response;
                    }
                }
            }
        }

        public enum HttpMethod
        {
            Get, Post, Put, Delete, Patch
        }

        public async Task<T> GetItem<T>(string url, T model, HttpMethod type = HttpMethod.Get, string token = "")
        {
            HttpResponseMessage httpResponseMessage;
            try
            {
                httpResponseMessage = await HttpRequestAsync<T>(url, type, token, model);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var content = await httpResponseMessage.Content.ReadAsStringAsync();
                    var Item = JsonConvert.DeserializeObject<T>(content);
                    return Item;
                }

            }
            catch (Exception x)
            {
                await _logger.InsertLogAsync(Core.Domain.Logging.LogLevel.Debug, "Inside getItem", x.Message);
                return model;
            }
            throw new Exception(httpResponseMessage.ReasonPhrase);
        }

        public async Task<string> GetToken()
        {
            //var log = new LoginApiModel()
            //{
            //    username = "AccelStg",
            //    password = "mS2.wN5!"
            //};
            var client = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(new LoginApiModel { username = "AccelStg", password = "mS2.wN5!" }));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync("https://peds.buyparts.biz/api/login", content);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var values = response.Headers.GetValues("X-AUTH-TOKEN").FirstOrDefault();
                return values;
            }
            return string.Empty;
        }

        #endregion

        #endregion

        #region Methods

        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            try
            {
                var token = await GetToken();

                //Get And Insert Years
                var years = await GetItem<List<YearModel>>($"https://peds.buyparts.biz/api/ymme/years", null, HttpMethod.Get, token);
                foreach (var year in years)
                {
                    var getYear = (await _productService.GetAllYearsAsync(yearId: year.id)).ToList();
                    if (getYear.Count > 0)
                        continue;

                    await _productService.InsertYearAsync(new Nop.Core.Domain.Catalog.Year
                    {
                        CreatedOn = DateTime.Now,
                        Deleted = false,
                        Name = year.value,
                        YearId = year.id
                    });

                    //Get And Insert Make
                    var makes = await GetItem<List<MakeModel>>($"https://peds.buyparts.biz/api/ymme/makes?yearId=" + year.id, null, HttpMethod.Get, token);
                    foreach (var make in makes)
                    {
                        var getMake = (await _productService.GetAllMakesAsync(makeId: make.id)).ToList();
                        if (getMake.Count > 0)
                            continue;

                        await _productService.InsertMakeAsync(new Make
                        {
                            CreatedOn = DateTime.Now,
                            Deleted = false,
                            Name = make.value,
                            MakeId = make.id,
                            YearId = year.id
                        });

                        //Get And Insert Model
                        var models = await GetItem<List<ModelModel>>($"https://peds.buyparts.biz/api/ymme/models?makeId=" + make.id + "&yearId=" + year.id, null, HttpMethod.Get, token);
                        foreach (var model in models)
                        {
                            var getModel = (await _productService.GetAllModelsAsync(modelId: model.id)).ToList();
                            if (getModel.Count > 0)
                                continue;

                            await _productService.InsertModelAsync(new Nop.Core.Domain.Catalog.Model
                            {
                                CreatedOn = DateTime.Now,
                                Deleted = false,
                                Name = model.value,
                                ModelId = model.id,
                                MakeId = make.id,
                                YearId = year.id,
                            });

                            //Get And Insert Engine
                            var engines = await GetItem<List<EngineModel>>($"https://peds.buyparts.biz/api/ymme/engines?makeId=" + make.id + "&modelId=" + model.id + "&yearId=" + year.id, null, HttpMethod.Get, token);
                            foreach (var engine in engines)
                            {
                                var getEngine = (await _productService.GetAllEnginesAsync(engineId: model.id)).ToList();
                                if (getEngine.Count > 0)
                                    continue;

                                await _productService.InsertEngineAsync(new Engine
                                {
                                    CreatedOn = DateTime.Now,
                                    Deleted = false,
                                    Name = engine.value,
                                    MakeId = make.id,
                                    YearId = year.id,
                                    EngineId = model.id
                                });
                            }
                        }
                    }
                }

                // Get And Insert Categories
                var categorys = await GetItem<List<ApiCategory>>($"https://peds.buyparts.biz/api/pti/all-part-categories/en?countryId=2", null, HttpMethod.Get, token);
                foreach (var category in categorys)
                {
                    var getCategory = await _categoryService.GetCategoryByCategoryApiIdAsync(category.id);
                    if (getCategory is null)
                    {
                        var newCategory = new Category
                        {
                            CreatedOnUtc = DateTime.UtcNow,
                            Deleted = false,
                            Name = category.name,
                            ApiCategoryId = category.id,
                            UpdatedOnUtc = DateTime.UtcNow,
                            Published = true,
                            CategoryTemplateId = 1,
                            PageSizeOptions = "6, 3, 9",
                            PageSize = 6
                        };
                        await _categoryService.InsertCategoryAsync(newCategory);

                        // Get And Insert Part Groups
                        foreach (var group in category.partGroups)
                        {
                            var getGroup = await _categoryService.GetAllPartGroupsAsync(apiPartGroupId: group.id);
                            if (getGroup.Count > 0)
                                continue;

                            var partGroup = new Core.Domain.Catalog.PartGroup
                            {
                                CreatedOn = DateTime.UtcNow,
                                Deleted = false,
                                Name = group.name,
                                EngineCode = group.engineCode,
                                ApiPartGroupId = group.id,
                            };
                            await _categoryService.InsertPartGroupAsync(partGroup);

                            //Insert Category part group
                            await _categoryService.InsertCategoryPartGroupAsync(new CategoryPartGroup
                            {
                                CategoryId = newCategory.Id,
                                PartGroupId = partGroup.Id
                            });

                            // Get And Insert Part Types
                            foreach (var type in group.partTypes)
                            {
                                var getType = await _categoryService.GetAllPartTypesAsync(apiPartTypeId: type.id);
                                if (getType.Count > 0)
                                    continue;

                                var partType = new Core.Domain.Catalog.PartType
                                {
                                    CreatedOn = DateTime.UtcNow,
                                    Deleted = false,
                                    Name = type.name,
                                    ApiPartTypeId = group.id,
                                    GroupId = partGroup.Id
                                };
                                await _categoryService.InsertPartTypeAsync(partType);
                            }
                        }

                    }
                    else
                    {
                        // Get And Insert Part Groups
                        foreach (var group in category.partGroups)
                        {
                            var getGroup = await _categoryService.GetAllPartGroupsAsync(apiPartGroupId: group.id);
                            if (getGroup.Count == 0)
                            {
                                var partGroup = new Core.Domain.Catalog.PartGroup
                                {
                                    CreatedOn = DateTime.UtcNow,
                                    Deleted = false,
                                    Name = group.name,
                                    EngineCode = group.engineCode,
                                    ApiPartGroupId = group.id,
                                };
                                await _categoryService.InsertPartGroupAsync(partGroup);

                                //Insert Category part group
                                await _categoryService.InsertCategoryPartGroupAsync(new CategoryPartGroup
                                {
                                    CategoryId = getCategory.Id,
                                    PartGroupId = partGroup.Id
                                });

                                // Get And Insert Part Types
                                foreach (var type in group.partTypes)
                                {
                                    var getType = await _categoryService.GetAllPartTypesAsync(apiPartTypeId: type.id);
                                    if (getType.Count > 0)
                                        continue;

                                    var partType = new Core.Domain.Catalog.PartType
                                    {
                                        CreatedOn = DateTime.UtcNow,
                                        Deleted = false,
                                        Name = type.name,
                                        ApiPartTypeId = group.id,
                                        GroupId = partGroup.Id
                                    };
                                    await _categoryService.InsertPartTypeAsync(partType);
                                }
                            }
                            else
                            {
                                // Get And Insert Part Types
                                foreach (var type in group.partTypes)
                                {
                                    var getType = await _categoryService.GetAllPartTypesAsync(apiPartTypeId: type.id);
                                    if (getType.Count > 0)
                                        continue;

                                    var partType = new Core.Domain.Catalog.PartType
                                    {
                                        CreatedOn = DateTime.UtcNow,
                                        Deleted = false,
                                        Name = type.name,
                                        ApiPartTypeId = group.id,
                                        GroupId = getGroup.FirstOrDefault().Id
                                    };
                                    await _categoryService.InsertPartTypeAsync(partType);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
    public class LoginApiModel
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class YearModel
    {
        public int id { get; set; }
        public string value { get; set; }
    }
    public class MakeModel
    {
        public int id { get; set; }
        public string value { get; set; }
    }
    public class ModelModel
    {
        public int id { get; set; }
        public string value { get; set; }
    }
    public class EngineModel
    {
        public int id { get; set; }
        public string value { get; set; }
    }
    public class PartType
    {
        public string id { get; set; }
        public string name { get; set; }
        public string groupId { get; set; }
    }
    public class PartGroup
    {
        public string id { get; set; }
        public string name { get; set; }
        public string engineCode { get; set; }
        public List<PartType> partTypes { get; set; }
    }

    public class ApiCategory
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<PartGroup> partGroups { get; set; }
    }
}