RegisterCommand('ui', function()
	SendNUIMessage({
		type = "showui",
		dto = {
			hello = "",
			message = ""
		}
	})

    TriggerEvent("showui")
end)