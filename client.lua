RegisterCommand('ui', function()
	SendNUIMessage({
		type = "showui:hello",
		hello = "before",
	})
end)

RegisterNUICallback('getItemInfo', function(data, cb)
	Wait(400)
	SendNUIMessage({
		type = "showui:hello",
		hello = "before",
	})
end)
