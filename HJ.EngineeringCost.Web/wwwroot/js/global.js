Date.prototype.Format = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "h+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
};

//表单转Object
$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

//显示图标
function showIcon(icon) {
    return "<i class='ok-icon'>" + icon + "</i>";
}

//时间戳转时间
function transferDatetime(str) {
    if (str == '/Date(-62135596800000)/') return '';
    var d = eval('new ' + str.substr(1, str.length - 2));
    return d.Format("yyyy-MM-dd hh:mm:ss");
}

/**
 * 四舍五入,保留小数位
 * @param {any} v
 * @param {any} n
 */
function mathRound(v, n) { return Math.round(v * Math.pow(10, n)) / Math.pow(10, n); }

function GetFloat(str) {
    if ($.trim(str) == "") {
        return 0;
    }
    return parseFloat(str);
}

function IsNullOrEmpty(str) {
    return str == null || str == "";
}

/**
 * input输入大于0的整数,至多9位数
 * @param {any} obj
 */
function inputNumber(obj, maxLength) {
    obj.value = obj.value.replace(/\D|^0/g, '');//只能输入整数
    if (maxLength > 0) {
        obj.value = obj.value.substring(0, maxLength);
    }
    if (IsNullOrEmpty(obj.value)) {
        obj.value = '0';
    }
}

/**
 * 限制input输入,保留两位小数
 * @param {any} obj
 */
function inputKeep2(obj) {
    obj.value = obj.value.replace(/^0*(0\.|[1-9])/, '$1');//解决 粘贴不生效
    obj.value = obj.value.replace(/[^\d.]/g, "");  //清除“数字”和“.”以外的字符
    obj.value = obj.value.replace(/\.{2,}/g, "."); //只保留第一个. 清除多余的
    obj.value = obj.value.replace(".", "$#$").replace(/\./g, "").replace("$#$", ".");
    obj.value = obj.value.replace(/^(\-)*(\d+)\.(\d\d).*$/, '$1$2.$3');//只能输入两个小数
}
