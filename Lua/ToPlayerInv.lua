function SetPlayer (player)
    local slot = players[player]
    if(slot) then
        turtle.select(slot)
        turtle.drop()
    else
        print("Error ", player, " is not in the system")
        return false
    end
    return true
end

function RemovePlayer ()
    turtle.suck()
end

function GetPlayers ()
    playerList = {}
    for i = 1, 16 do
        local slot = turtle.getItemDetail(i, true)
        if slot then
            local name = slot.displayName
            playerList[name] = i
        end
    end
    return playerList
end

function StartProcess (list)
    if(SetPlayer(list.Player)) then
        if(list.Direction) then
            GiveItems(list.Items)
        else
            StoreItems(list.Items)
        RemovePlayer()
    end
end

function GiveItems(items)
    local manager = peripheral.find("inventoryManager")
    for i, item in pairs(items) do
        --print(item.Qty, item.Name)
        manager.addItemToPlayer("left", item.Qty, nil, item.Name)
        
    end
end

function StoreItems(items)
    local manager = peripheral.find("inventoryManager")
    for i, item in pairs(items) do
        --print(item.Qty, item.Name)
        manager.removeItemFromPlayer("left", item.Qty, nil, item.Name)
        
    end
end

peripheral.find("modem", rednet.open)
run = true
players = GetPlayers()
while(run) do
    local id, message
    repeat
        id, message = rednet.receive("main")
        --send some reply that it was succesfull
    until message ~= 0
    players = GetPlayers()
    list = textutils.unserialiseJSON(message)
    StartProcess(list)
    --Send some reply with how succesfull it was
end
peripheral.find("modem", rednet.close)