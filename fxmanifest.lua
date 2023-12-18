version '1.0.0'
author 'Twinki'
description 'A BlazorClient in FiveM'

ui_page 'src/Blazor-UI/bin/Release/net8.0/publish/wwwroot/index.html'

files {
  'src/Blazor-UI/bin/Release/net8.0/publish/wwwroot/**/*'
}

fx_version 'adamant'
games { 'gta5' }