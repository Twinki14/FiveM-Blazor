RegisterCommand('ui', function()
	SendNUIMessage({
		type = "showui:hello",
		hello = "test",
		time = GetClockMinutes(),
		dto = {
			hello = "",
			message = ""
		}
	})
end)
