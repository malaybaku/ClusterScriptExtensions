// @field(string), @textArea
const myLongText = ""

// @field(int), @range(-10, 20)
const myRangeInt = -1;

// @field(float), @range(0.1, 5.0)
const myRangeFloat = 1.0;

// NOTE: enumはまだ対応できていない
// @field(int), @enum("OptionA", "2つ目のオプション", "No.3")
const myEnum = 0;

$.onInteract(() => { 
  $.log(`long text... ${myLongText}`);

  $.log(`range int... ${myRangeInt}`);
  $.log(`range float... ${myRangeFloat}`);

  $.log(`enum... ${myEnum}`);
});
