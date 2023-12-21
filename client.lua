RegisterCommand('ui', function()
	SendNUIMessage({
		type = "showui:hello",
		hello = 14,
		dto = {
			hello = "",
			message = ""
		}
	})

    TriggerEvent("showui")
end)