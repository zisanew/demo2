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

function getModifiedFields(original, current) {
    let modified = {};
    for (let key in current) {
        if (current.hasOwnProperty(key)) {
            const originalValue = original[key];
            const currentValue = current[key];

            // 检查是否为 null 或 undefined
            if (originalValue === null || originalValue === undefined || currentValue === null || currentValue === undefined) {
                if (originalValue !== currentValue) {
                    modified[key] = { original: originalValue, current: currentValue };
                }
            } else {
                if (String(originalValue).trim() !== String(currentValue).trim()) {
                    modified[key] = { original: originalValue, current: currentValue };
                }
            }
        }
    }
    return modified;
}

function formatModifiedFields(modifiedFields) {
    return Object.keys(modifiedFields).map(key => {
        const inputElement = $(`input[name="${key}"]`);
        const plFormLine = inputElement.closest('.pl-form-line');
        const labelElement = plFormLine.find('.layui-form-label');
        const label = labelElement.text().trim() || key;
        return `<strong>${label}</strong>: ${modifiedFields[key].original} => ${modifiedFields[key].current}`;
    }).join('<br>');
}

function getGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        const r = (Math.random() * 16) | 0;
        const v = c === 'x' ? r : (r & 0x3) | 0x8; // 确保 y 的值在 8-11 之间
        return v.toString(16);
    });
}

/**
 * 导出Excel
 * @param {any} layui
 * @param {any} url 数据接口
 * @param {any} where 条件
 * @param {any} method 方法
 * @param {any} colsItem 头部表头绑定列
 * @param {any} fieldArr 表头列【用于排序】
 * @param {any} filesign 文件标志
 */
function LAYExportExcel(layui, url, where, method, colsItem, fieldArr, filesign) {
	let excel = layui.excel;
	let okLayer = layui.okLayer;
	where.page = 1;
	where.limit = 10001;//导出数据上限+1

	$.ajax({
		url: url,
		type: method,
		data: where,
		success: function (res) {
            if (res.total == 0) {
				okLayer.yellowSighMsg("根据条件筛选后，没有可供导出的数据", null, 1500);
				return false;
			}
            else if (res.total > 10000) {
				layer.confirm('根据条件导出一次最多10000条数据，是否导出？', function (index) {
					layer.close(index);
					res.data.splice(10000, 1);//删除第10001条数据
					doLAYExportExcel(excel, res.data, colsItem, fieldArr, filesign);
				});
			} else {
				doLAYExportExcel(excel, res.data, colsItem, fieldArr, filesign);
			}
		},
	});
}

function doLAYExportExcel(excel, data, colsItem, fieldArr, filesign) {
	// 1.数组头部新增表头 { name: '名字', age: '年龄' }
	data.unshift(colsItem);
	// 2.数据格式自定义转换
	let file = getFileInfo(filesign, data);
	// 3.如果需要调整顺序，请执行梳理函数 ['name','age']
	let sheetData = excel.filterExportData(file[1], fieldArr);
	// 4.执行导出函数，系统会弹出弹框
	excel.exportExcel({ sheet1: sheetData }, file[0].concat(".xlsx"), "xlsx");
}

function getFileInfo(filesign, data) {
	let file = [];
	switch (filesign) {
        case "edi830":
			file[0] = "EDI830";
			file[1] = data;
            break;
        case "edi850":
            file[0] = "EDI850";
            file[1] = data;
            break;
	}
	return file;
}