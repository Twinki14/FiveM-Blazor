version '1.0.0'
author 'Twinki'
description 'A BlazorClient in FiveM'

ui_page 'src/BlazorUI/bin/Release/net8.0/publish/wwwroot/index.html'

client_scripts {
  'client.lua'
}

files {
  'src/BlazorUI/bin/Release/net8.0/publish/wwwroot/**/*'
}

fx_version 'adamant'
games { 'gta5' }