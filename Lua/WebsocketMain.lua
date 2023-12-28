function listPlayer (player)
    local i = 1
    local list = {}
    players = GetPlayers()
    SetPlayer(player)
    local inv = invManager.getItems()
    for slot, item in pairs(inv) do
        list[i] = {name = item.name, count = item.count, slot = slot}
        i = i + 1
    end
    RemovePlayer();
    return list
end

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

function broadCast(payload)
    
    chatBox.sendMessage(payload, "Compooter")
end

chatBox = peripheral.find("chatBox")
invManager = peripheral.find("inventoryManager")
turtle.suck()
--local inv = peripheral.find("inventory")
--chestData = listChest(inv)
--rednet.open("top")
players = GetPlayers()
while(true) do
    local ws, err = assert(http.websocket("wss://chestapp.azurewebsites.net/ws"))
    if(not ws) then print(err) else print("Success") end
    ws.send("Main")
    while(ws) do
        local reply, binary = ws.receive(10)
        replyb = string.byte(reply)
        if(replyb) then print("Reply: " .. replyb) else print("Null") end
        os.sleep(2)
        if(replyb == 0 or replyb == 2) then
            ws.send(0, true)
        elseif(replyb == 1) then
            --ws.send(textutils.serialiseJSON(listChest(inv)))
        else
            --=broadCast(reply);
            ws.send(textutils.serialiseJSON(listPlayer(reply)))
            
        end
    end
    if(ws) then ws.close() end
end

