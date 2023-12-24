RegisterCommand('ui', function()
	SendNUIMessage({
		type = "showui:hello",
		hello = "before",
	})
end)

RegisterNUICallback('getItemInfo', function(data, cb)
	Wait(2000)
	SendNUIMessage({
		type = "showui:hello",
		hello = "after",
	})
end)
