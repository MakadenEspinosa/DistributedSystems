# VideoGameExchange.Client.Api.DefaultApi

All URIs are relative to *https://virtserver.swaggerhub.com/neumontuniversity/VideoGameExchange/1.0.0*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**AddGame**](DefaultApi.md#addgame) | **POST** /games | Create a new game to add to your user |
| [**DeleteGame**](DefaultApi.md#deletegame) | **DELETE** /games/{gameid} | Delete a game from the user&#39;s collection |
| [**PartialGameUpdate**](DefaultApi.md#partialgameupdate) | **PATCH** /games/{gameid} | Partially update a game |
| [**UpdateGame**](DefaultApi.md#updategame) | **PUT** /games/{gameid} | Update a game in your user&#39;s collection |

<a id="addgame"></a>
# **AddGame**
> Game AddGame (GameInput gameInput)

Create a new game to add to your user


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **gameInput** | [**GameInput**](GameInput.md) |  |  |

### Return type

[**Game**](Game.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **201** | Game added successfully |  * Location - URL of the newly created game resource <br>  |
| **400** | Bad Request |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="deletegame"></a>
# **DeleteGame**
> void DeleteGame (string gameid)

Delete a game from the user's collection


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **gameid** | **string** | The ID of the game |  |

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **204** | Deleted game successfully |  -  |
| **404** | Game not found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="partialgameupdate"></a>
# **PartialGameUpdate**
> Game PartialGameUpdate (string gameid, PatchGame patchGame)

Partially update a game


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **gameid** | **string** | The ID of the game |  |
| **patchGame** | [**PatchGame**](PatchGame.md) |  |  |

### Return type

[**Game**](Game.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/merge-patch+json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Game updates successful |  -  |
| **400** | Bad request |  -  |
| **404** | Game not found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="updategame"></a>
# **UpdateGame**
> Game UpdateGame (string gameid, GameInput gameInput)

Update a game in your user's collection


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **gameid** | **string** | The ID of the game |  |
| **gameInput** | [**GameInput**](GameInput.md) |  |  |

### Return type

[**Game**](Game.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Game updated successfully |  -  |
| **400** | Bad Request |  -  |
| **404** | Game not found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

