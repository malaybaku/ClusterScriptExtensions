// string, int, float のインスペクターで 複数行での表示や min/max つきスライダーを提示できるサンプル。

// @field(string), @textArea
const myLongText = ""

// @field(int), @range(-10, 20)
const myRangeInt = -1;

// @field(float), @range(0.1, 5.0)
const myRangeFloat = 1.0;

$.onInteract(() => { 
  showLog(
    `long text... ${myLongText}\n` + 
    `range int... ${myRangeInt}\n` + 
    `range float... ${myRangeFloat.toFixed(3)}`
  );
});

const showLog = (text) => { 
  $.subNode("LogText").setText(text);
  $.log(text);
}
