"use strict";

if (typeof window.LivecodingTV === "undefined") {
    window.LivecodingTV = {};
}

(function ($, Visibility, Mustache, LivecodingTV) {
    if (typeof Array.isArray === 'undefined') {
        Array.isArray = function(obj) {
            return Object.prototype.toString.call(obj) === "[object Array]";
        }
    }
    var isRegExp = function(obj) {
        return Object.prototype.toString.call(obj) === "[object RegExp]";
    };
    var isNumeric = function(obj) {
        return (typeof obj === "number") || (typeof obj === "string" && obj.match(/^-?\d+(\.\d+)?$/));
    };

    var ___ = {},
        FlatArrayPattern = function(pattern) { this.pattern = pattern; },
        F = function() { return new FlatArrayPattern(Array.prototype.slice.call(arguments)); };

    function matchPattern(pattern, data) {
        var matches = [], submatches;

        if (pattern.length !== data.length) { return null; }
        for (var i = 0; i < data.length; i++) {
            var match = pattern[i],
                item = data[i];
            if (match === ___) { matches.push(item) }
            else if (match === String && typeof item === "string") { matches.push(item); }
            else if (match === Number && typeof item === "number") { matches.push(item); }
            else if (match === Boolean && typeof item === "boolean") { matches.push(item); }
            else if (match === Array && Array.isArray(item)) { matches.push(item); }
            else if (isRegExp(match) && typeof item === "string") {
                var m = item.match(match);
                if (m !== null) {
                    m = m.slice(1);
                    if (m.length === 1) {
                        matches.push(m[0]);
                    } else if (m.length > 0) {
                        matches.push(m);
                    } else {
                        matches.push(item);
                    }
                } else {
                    return null;
                }
            }
            else if (match instanceof FlatArrayPattern && Array.isArray(item)) {
                submatches = matchPattern(match.pattern, item);
                if (submatches !== null) { matches.push.apply(matches, submatches); } else { return null; }
            }
            else if (Array.isArray(match) && Array.isArray(item)) {
                submatches = matchPattern(match, item);
                if (submatches !== null) { matches.push(submatches); } else { return null; }
            }
            else if (match !== item) { return null; }
        }
        return matches;
    }

    function comparePatterns(a1, a2) {
        if (a1.length != a2.length)
            return false;

        for (var i = 0, l = a1.length; i < l; i++) {
            if (a1[i] instanceof Array && a2[i] instanceof Array) {
                if (!comparePatterns(a1[i], a2[i]))
                    return false;
            }
            else if (a1[i] instanceof FlatArrayPattern && a2[i] instanceof FlatArrayPattern) {
                if (!comparePatterns(a1[i].pattern, a2[i].pattern))
                    return false;
            }
            else if (a1[i] !== a2[i]) {
                return false;
            }
        }
        return true;
    }

    function match(data, patterns, nomatch) {
        if (patterns.length % 2 != 0) {
            throw new Error("match: patterns must contain even number of items (pattern, handler)");
        }
        // Currently, we do plain linear search -- simple to implement and fast enough.
        // If there would be a lot of patterns and messages, so this would get unacceptably slow,
        // consider compiling (or, more likely, pre-compiling -- you don't want to run this every
        // time the page loads) a DFA to do the matching.
        for (var i = 0; i < patterns.length - 1; i += 2) {
            var pattern = patterns[i],
                handler = patterns[i + 1];

            var m = matchPattern(pattern, data);
            if (m !== null) {
                return handler.apply(data, m);
            }
        }
        if (typeof nomatch === "function") {
            return nomatch(data);
        } else {
            return nomatch;
        }
    }

    jQuery.fn.fadeOutAndRemove = function(speed){
        $(this).fadeOut(speed,function(){
            $(this).remove();
        })
    };

    var notificationTemplate = $("#notification_template").html(),
        hiddenNotificationClasses = [];
    Mustache.parse(notificationTemplate); // Speed up future uses.
    function showNotification(message, options) {
        options = options || {};

        var classes = [];
        if (options.cls) {
            if (!Array.isArray(options.cls)) {
                classes = [options.cls]
            } else {
                classes = options.cls;
            }
            for (var i = 0; i < classes.length; i++) {
                if ($.inArray(classes[i], hiddenNotificationClasses) >= 0) {
                    // Ignore this notification
                    console.log("Ignoring notification with class ", classes[i]);
                    return false;
                }
            }
        }

        if (typeof Visibility !== "undefined" && !Visibility.hidden()) {
            var $notifications = $("#notifications-popup-container");
            if ($notifications.length < 1) {
                $notifications = $("<div>").attr("id", "notifications-popup-container").appendTo(document.body);
            }
            // On .slice() vs ":gt()" differences: http://stackoverflow.com/a/10967977/116546
            $notifications.children(".notification-live").slice(1).fadeOutAndRemove("slow");
            var $notification = $(Mustache.render(notificationTemplate, $.extend(options, {
                message: message,
                classes: classes,
                icon: options.icon || null
            })));
            console.log($notification, options);
            $notification.find(".close").click(function(e) {
                e.stopPropagation();
                e.preventDefault();
                $notification.fadeOutAndRemove("fast");
            });
            if (classes) { $notification.addClass(classes); }
            if (options.url) {
                var url = options.url;
                $notification.addClass("link");
                $notification.click(function () {
                    window.location.assign(url);
                    $notification.fadeOutAndRemove("fast");
                });
            }
            $notification.hide().prependTo($notifications).fadeIn("fast")
        } else {
            // Page's not visible. Use desktop notifications.
            if (Notification.permission === "granted") {
                var n = new Notification(message);
                n.onclick = function () {
                    window.focus();
                    this.close();
                };
            } else if (Notification.permission !== "denied") {
                Notification.requestPermission(function (permission) {
                    if (permission === "granted") {
                        var n = new Notification(message);
                        n.onclick = function () {
                            window.focus();
                            this.close();
                        };
                    }
                });
            }
        }
    }

    if (typeof eio !== "undefined") {
		console.log("LIVE ADDRESS: " + LivecodingTV.config.liveAddress);
        var socket = eio(LivecodingTV.config.liveAddress, {path: "/live.eio"}),
            channels = {},
            enableReconnect = false;
			
		socket.__send = socket.send;
		socket.send = function(data) {
			console.log("Sending Data: ", data);
			socket.__send(data);
		}
		
		socket.__on = socket.on;
		socket.on = function(evt, func) {
			console.log("Registered event(" + evt + ")", func);
			socket.__on(evt, func);
		}

        LivecodingTV.live = {
            // Public API bits.
            open: function () { socket.open(); },
            close: function () { enableReconnect = false; socket.close(); },
            send: function (data) { socket.send(JSON.stringify(data)); },
            join: function (name) {
                if (socket.readyState === "open") {
                    socket.send(JSON.stringify(["join", name]));
                } else {
                    channels[name] = true;
                }
            },
            leave: function (name) { socket.send(JSON.stringify(["leave", name])); },
            registerPattern: function (pattern, handler) {
                var p = LivecodingTV.live._patterns;
                for (var i = 0; i < p.length - 1; i += 2) {
                    if (comparePatterns(p[i], pattern)) {
                        console.log("bug: tried to re-register already known pattern", pattern);
                        return false;
                    }
                }
                p.push(pattern);
                p.push(handler);
                return true;
            },
            channels: channels,
            hideNotifications: function (cls) {
                if ($.inArray(cls, hiddenNotificationClasses) < 0) {
                    hiddenNotificationClasses.push(cls);
                }
            },
            debug: true,  // Enables logging of raw messages to JS console.

            // Private API bits. Exposed to aid debugging, but don't rely on those.
            _socket: socket,
            _patterns: []
        };

        socket.on("open", function () {
            console.log("live: open");
            enableReconnect = true;
            for (var channel in channels) {
                socket.send(JSON.stringify(["join", channel]));
            }
        });
		socket.on("packet", function (packet) {
			console.log("Recieved Packet: ", packet);
		});
        socket.on("message", function (messageData) {
            try {
                messageData = JSON.parse(messageData);
                if (LivecodingTV.live.debug) {
                    console.log("live: raw message:", messageData);
                }

                match(messageData, [
                    ["user_msg", F("new_messages", Boolean)], function (flag) {
                        $("#topSidebar").find(".top-right-nav li.messages a.letter").toggleClass("new-messages", flag);
                    },
                    ["user_msg", F("show", String)], function (text) {
                        showNotification(text);
                    },
                    ["user_msg", F("show", String, ___)], function (text, options) {
                        showNotification(text, options);
                    },
                    ["user_msg", ___], function (message) {
                        console.log("unknown user message", message);
                    },
                    ["channel_msg", /^stream\.(\w+)$/, ___], function (channel, message) {
                        console.log("stream", channel, message);
                        if (isNumeric(message.views_live)) {
                            $("#views_live").text(message.views_live);
                        }
                        if (isNumeric(message.views_overall)) {
                            $("#views_overall").text(message.views_overall);
                        }
                    },
                    ["channel_msg", String, ___], function (channel, message) {
                        console.log("unknown channel message", channel, message);
                    },
                    [/^(join|leave)$/, String], function (action, channel) {
                        if (action === "join") {
                            channels[channel] = true;
                        } else {
                            delete channels[channel];
                        }
                    },
                    ["bye"], function () {
                        enableReconnect = false;
                        socket.close();
                    }
                ].concat(LivecodingTV.live._patterns), function (data) {
                    console.log("unknown message", data);
                });
            } catch(e) {
                console.log(e);
            }
        });
        socket.on("close", function () {
            console.log("live: closed");
            if (enableReconnect) {
                setTimeout(function () { socket.open(); }, 1000);
            }
        });
    }
})(jQuery, Visibility, Mustache, window.LivecodingTV);
