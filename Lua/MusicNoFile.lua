function mysplit (inputstr, sep)
  if sep == nil then
          sep = "%s"
  end
  local t={}
  for str in string.gmatch(inputstr, "([^"..sep.."]+)") do
          table.insert(t, str)
  end
  return t
end


local dfpwm = require("cc.audio.dfpwm")
local speakers = { peripheral.find("speaker") }
local volume = 50
local decoder = dfpwm.make_decoder()
local song = "Song"
local link = mysplit(arg[1], "v=")[2]
local fileDownload, reason = http.get("https://chestapp.azurewebsites.net/Music/convert/" .. link, nil, true)
local code = fileDownload.
while(code = "")do

end

local data = fileDownload.readAll()
while(true)
do
for i = 1, 1, 1
do
  for i = 1, #data, 16*64 do
    local buffer = decoder(data:sub(i,i+16*64-1))

      while not speakers[1].playAudio(buffer, volume) do
          os.pullEvent("speaker_audio_empty")
      end
    end
end

end
