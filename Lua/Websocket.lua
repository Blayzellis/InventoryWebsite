function listChest (chest)
    local i = 1
    local list = {}
    for slot, item in pairs(chest.list()) do
        list[i] = {name = item.name, count = item.count, slot = slot}
        i = i + 1
    end
    return list
end

local inv = peripheral.find("inventory")
chestData = listChest(inv)
rednet.open("top")
while(redstone.getInput("left")) do
    local ws, err = assert(http.websocket("ws://162.231.184.144:8080/ws"))
    if(not ws) then print(err) else print("Success") end
    ws.send("main")
    while(ws and redstone.getInput("left")) do
        local reply, binary = ws.receive(5)
        replyb = string.byte(reply)
        if(replyb) then print("Reply: " .. replyb) else print("Null") end
        os.sleep(1)
        if(replyb == 0 or replyb == 2) then
            ws.send(0, true)
        elseif(replyb == 1) then
            ws.send(textutils.serialiseJSON(listChest(inv)))
        else
            ws.send(0, true)
            rednet.broadcast(reply, "main")
        end
    end
    if(ws) then ws.close() end
end

