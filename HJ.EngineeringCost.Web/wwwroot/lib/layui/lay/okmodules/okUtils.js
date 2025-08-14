"use strict";
layui.define(["layer"], function (exprots) {
    var $ = layui.jquery;
    var okUtils = {
        /**
         * 获取body的总宽度
         */
        getBodyWidth: function () {
            return document.body.scrollWidth;
        },
        /**
         * ajax()函数二次封装
         * @param url - 请求的 URL
         * @param type - 请求类型 (GET, POST, PUT, DELETE 等)
         * @param params - 请求参数
         * @param load - 是否显示加载动画
         * @param headers - 自定义请求头对象
         * @returns {*|never|{always, promise, state, then}}
         */
        ajax: function (url, type, params, load, headers, businessErrorHandle) {
            var deferred = $.Deferred();
            var loadIndex;

            // 默认请求头
            var defaultHeaders = {};

            // 如果提供了自定义请求头，则合并到默认请求头中
            if (headers) {
                defaultHeaders = $.extend(defaultHeaders, headers);
            }

            // 如果是 POST、PUT、DELETE 请求，自动添加 Anti-Forgery Token
            if (['POST', 'PUT', 'DELETE'].includes((type || "get").toUpperCase())) {
                var antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();
                if (antiForgeryToken) {
                    defaultHeaders['RequestVerificationToken'] = antiForgeryToken;
                }
            }

            $.ajax({
                url: url,
                type: type || "get",
                data: type.toUpperCase() === 'GET' ? params : JSON.stringify(params),
                contentType: type.toUpperCase() === 'GET' ? '' : 'application/json',
                dataType: "json",
                headers: defaultHeaders, // 添加请求头
                beforeSend: function () {
                    if (load) {
                        loadIndex = layer.load(0, { shade: false });
                    }
                },
                success: function (data, textStatus, jqXHR) {
                    if (data.code == 0 || businessErrorHandle) {
                        // 业务正常
                        var refreshAccessToken = jqXHR.getResponseHeader('Authorization');
                        if (refreshAccessToken) {
                            // 更新 token
                            localStorage.setItem('X-MiniAuth-Token', refreshAccessToken);
                        }
                        deferred.resolve(data);
                    } else {
                        // 业务异常
                        //layer.msg(data.message, { icon: 7, time: 2000 });
                        let options = { icon: 2, title: "错误提示" };
                        layer.confirm(data.message, options);
                    }
                },
                complete: function () {
                    if (load) {
                        layer.close(loadIndex);
                    }
                },
                error: function (xhr, status, error) {
                    layer.close(loadIndex);
                    if (xhr.status === 401) {
                        // 处理 401 响应
                        alert('未授权，请登录');
                        window.location.href = xhr.responseJSON.redirectTo;
                    } else {
                        // 处理其他错误
                        console.error('Error:', error);
                    }
                }
            });

            return deferred.promise();
        },
        /**
         * ajax()函数二次封装
         * @param url - 请求的 URL
         * @param type - 请求类型 (GET, POST, PUT, DELETE 等)
         * @param params - 请求参数
         * @param load - 是否显示加载动画
         * @param headers - 自定义请求头对象
         * @returns {*|never|{always, promise, state, then}}
         */
        uploadFile: function (url, type, params, load, headers) {
            var deferred = $.Deferred();
            var loadIndex;

            // 默认请求头
            var defaultHeaders = {};

            // 如果提供了自定义请求头，则合并到默认请求头中
            if (headers) {
                defaultHeaders = $.extend(defaultHeaders, headers);
            }

            // 如果是 POST、PUT、DELETE 请求，自动添加 Anti-Forgery Token
            if (['POST', 'PUT', 'DELETE'].includes((type || "get").toUpperCase())) {
                var antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();
                if (antiForgeryToken) {
                    defaultHeaders['RequestVerificationToken'] = antiForgeryToken;
                }
            }

            // 创建 FormData 对象
            var formData = new FormData();
            if (params && typeof params === 'object') {
                for (var key in params) {
                    if (params.hasOwnProperty(key)) {
                        formData.append(key, params[key]);
                    }
                }
            }

            $.ajax({
                url: url,
                type: type || "get",
                data: formData,
                contentType: false,
                processData: false, // 禁用 processData 以避免 jQuery 自动转换数据
                dataType: "json",
                headers: defaultHeaders, // 添加请求头
                beforeSend: function () {
                    if (load) {
                        loadIndex = layer.load(0, { shade: false });
                    }
                },
                success: function (data, textStatus, jqXHR) {
                    if (data.code == 0) {
                        // 业务正常
                        var refreshAccessToken = jqXHR.getResponseHeader('Authorization');
                        if (refreshAccessToken) {
                            // 更新 token
                            localStorage.setItem('X-MiniAuth-Token', refreshAccessToken);
                        }
                        deferred.resolve(data);
                    } else {
                        // 业务异常
                        //layer.msg(data.message, { icon: 7, time: 2000 });
                        let options = { icon: 2, title: "错误提示" };
                        layer.confirm(data.message, options);
                    }
                },
                complete: function () {
                    if (load) {
                        layer.close(loadIndex);
                    }
                },
                error: function (xhr, status, error) {
                    layer.close(loadIndex);
                    if (xhr.status === 401) {
                        // 处理 401 响应
                        alert('未授权，请登录');
                        window.location.href = xhr.responseJSON.redirectTo;
                    } else {
                        // 处理其他错误
                        console.error('Error:', error);
                    }
                }
            });

            return deferred.promise();
        },
        /**
         * 下载文件
         * @param url - 请求的 URL
         * @param method 
         * @param params - 请求参数
         * @param load - 是否显示加载动画
         */
        downloadFile: function downloadFile(url, method, params, load) {
            var loadIndex;

            if (load) {
                loadIndex = layer.load(0, { shade: false });
            }

            const options = {
                method: method || "POST",
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() || ''
                }
            };

            if (params && Object.keys(params).length > 0) {
                options.body = JSON.stringify(params);
            }

            // 发送请求
            fetch(url, options)
                .then(response => {
                    if (!response.ok) {
                        if (response.status === 401 || response.status === 302) {
                            throw new Error('登录信息已失效，请重新登录');
                        } else {
                            throw new Error('网络响应失败');
                        }
                    }

                    // 检查响应URL是否指向登录页面
                    const redirectUrl = new URL(response.url);
                    if (redirectUrl.pathname.includes('/MiniAuth/login.html')) {
                        throw new Error('登录信息已失效，请重新登录');
                    }

                    const contentType = response.headers.get('Content-Type') || '';
                    if (contentType.includes('text/html')) {
                        return response.text().then(text => {
                            if (text.includes('/MiniAuth/logout')) {
                                throw new Error('登录信息已失效，请重新登录');
                            } else {
                                throw new Error('响应内容不是预期的文件类型');
                            }
                        });
                    }

                    const contentDisposition = response.headers.get('Content-Disposition');

                    //if (contentType && contentType.includes('application/vnd.openxmlformats-officedocument.spreadsheetml.sheet')) {
                    //    return response.blob().then(blob => ({ blob, response }));
                    //} else {
                    //    throw new Error('响应内容不是 Excel 文件');
                    //}

                    const isExcel = contentType.includes('application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
                    const isZip = contentType.includes('application/zip');
                    if (isExcel || isZip) {
                        return response.blob().then(blob => ({ blob, response }));
                    } else {
                        throw new Error('不支持的文件类型，仅支持 Excel 或 ZIP');
                    }
                })
                .then(data => {
                    const { blob, response } = data;
                    var filename = 'export.xlsx';
                    const contentDisposition = response.headers.get('Content-Disposition');
                    if (contentDisposition) {
                        const extractedFilename = getFilenameFromContentDisposition(contentDisposition);
                        if (extractedFilename) {
                            filename = extractedFilename;
                        }
                    }

                    // 创建一个 Blob 对象
                    var url = window.URL.createObjectURL(blob);

                    // 创建一个隐藏的 <a> 标签来触发文件下载
                    var a = document.createElement('a');
                    a.style.display = 'none';
                    a.href = url;
                    a.download = filename;  // 设置下载文件名

                    // 将 <a> 标签添加到 DOM 中并触发点击事件
                    document.body.appendChild(a);
                    a.click();

                    // 移除 <a> 标签并释放 URL
                    document.body.removeChild(a);
                    window.URL.revokeObjectURL(url);
                })
                .catch(error => {
                    console.error('Error:', error);  // 输出详细的错误信息
                    layer.msg("程序异常: " + error.message, { icon: 2, time: 2000 });
                })
                .finally(() => {
                    if (load) {
                        layer.close(loadIndex);
                    }
                });
        },
        /**
         * 主要用于针对表格批量操作操作之前的检查
         * @param table
         * @returns {string}
         */
        tableBatchCheck: function (table) {
            var checkStatus = table.checkStatus("tableId");
            var rows = checkStatus.data.length;
            if (rows > 0) {
                var idsStr = "";
                for (var i = 0; i < checkStatus.data.length; i++) {
                    idsStr += checkStatus.data[i].Id + ",";
                }
                return idsStr;
            } else {
                layer.msg("未选择有效数据", { offset: "t", anim: 6 });
            }
        },
        /**
         * 在表格页面操作成功后弹窗提示
         * @param content
         */
        tableSuccessMsg: function (content) {
            layer.msg(content, { icon: 1, time: 700 }, function () {
                // 刷新当前页table数据
                $(".layui-laypage-btn")[0].click();
            });
        },
        /**
         * 获取父窗体的okTab
         * @returns {string}
         */
        getOkTab: function () {
            return parent.objOkTab;
        },
        /**
         * 格式化当前日期
         * @param date
         * @param fmt
         * @returns {void | string}
         */
        dateFormat: function (date, fmt) {
            var o = {
                "M+": date.getMonth() + 1,
                "d+": date.getDate(),
                "h+": date.getHours(),
                "m+": date.getMinutes(),
                "s+": date.getSeconds(),
                "q+": Math.floor((date.getMonth() + 3) / 3),
                "S": date.getMilliseconds()
            };
            if (/(y+)/.test(fmt))
                fmt = fmt.replace(RegExp.$1, (date.getFullYear() + "").substr(4 - RegExp.$1.length));
            for (var k in o)
                if (new RegExp("(" + k + ")").test(fmt))
                    fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
            return fmt;
        },
        number: {
            /**
             * 判断是否为一个正常的数字
             * @param num
             */
            isNumber: function (num) {
                if (num && !isNaN(num)) {
                    return true;
                }
                return false;
            },
            /**
             * 判断一个数字是否包括在某个范围
             * @param num
             * @param begin
             * @param end
             */
            isNumberWith: function (num, begin, end) {
                if (this.isNumber(num)) {
                    if (num >= begin && num <= end) {
                        return true;
                    }
                    return false;
                }
            },
        }
    };

    function getFilenameFromContentDisposition(contentDisposition) {
        if (!contentDisposition) return null;

        const match = contentDisposition.match(/filename=["']?([^"'\r\n]*?)["']?(\s*;|$)/i);
        if (match && match[1]) {
            let filename = decodeURIComponent(match[1].trim());
            if (filename) {
                return filename;
            }
        }

        const encodedMatch = contentDisposition.match(/filename\*=([^\s;]+)/i);
        if (encodedMatch && encodedMatch[1]) {
            try {
                // 解码 URL 编码的字符串，并去除可能存在的引号
                let encodedFilename = decodeURIComponent(encodedMatch[1].trim().replace(/['"]/g, ''));

                if (encodedFilename.startsWith("UTF-8''")) {
                    encodedFilename = decodeURIComponent(encodedFilename.slice(7)); // 去除 "UTF-8''" 前缀
                }

                return encodedFilename;
            } catch (e) {
                console.warn('Failed to decode filename*:', e);
            }
        }

        return null;
    }

    exprots("okUtils", okUtils);
});
