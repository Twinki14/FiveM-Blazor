RegisterCommand('ui', function()
    SendNUIMessage({
        hello = "world",
        action = "showMessage"
    })

    TriggerEvent("eventName")
end)