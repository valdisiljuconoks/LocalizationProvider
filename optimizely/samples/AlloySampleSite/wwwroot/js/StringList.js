/*
Dojo widget for editing a list of strings. Also see property type PropertyStringList in /Models/Properties.
*/

define([
    "dojo/_base/array",
    "dojo/_base/connect",
    "dojo/_base/declare",
    "dojo/_base/lang",

    "dijit/_CssStateMixin",
    "dijit/_Widget",
    "dijit/_TemplatedMixin",
    "dijit/_WidgetsInTemplateMixin",

    "dijit/form/Textarea",

    "epi/epi",
    "epi/shell/widget/_ValueRequiredMixin"
],
function (
    array,
    connect,
    declare,
    lang,

    _CssStateMixin,
    _Widget,
    _TemplatedMixin,
    _WidgetsInTemplateMixin,

    Textarea,
    epi,
    _ValueRequiredMixin
) {

    return declare("alloy.editors.StringList", [_Widget, _TemplatedMixin, _WidgetsInTemplateMixin, _CssStateMixin, _ValueRequiredMixin], {

        templateString: "<div class=\"dijitInline\" tabindex=\"-1\" role=\"presentation\">\
                            <div data-dojo-attach-point=\"stateNode, tooltipNode\">\
                                <div data-dojo-attach-point=\"textArea\" data-dojo-type=\"dijit.form.Textarea\"></div>\
                            </div>\
                            <br />\
                            <span>${helptext}</span>\
                        </div>",

        baseClass: "epiStringList",

        helptext: "Place items on separate lines",

        intermediateChanges: false,

        value: null,

        multiple: true,

        onChange: function (value) {
            // Event
        },

        postCreate: function () {
            // call base implementation
            this.inherited(arguments);

            // Init textarea and bind event
            this.textArea.set("intermediateChanges", this.intermediateChanges);
            this.connect(this.textArea, "onChange", this._onTextAreaChanged);
        },

        isValid: function () {
            // summary:
            //    Check if widget's value is valid.
            // tags:
            //    protected, override

            return !this.required || lang.isArray(this.value) && this.value.length > 0 && this.value.join() !== "";
        },

        // Setter for value property
        _setValueAttr: function (value) {
            this._setValue(value, true);
        },

        _getValueAttr: function () {
            // summary:
            //   Returns the textbox value as array.
            // tags:
            //    protected, override

            var val = this.textArea && this.textArea.get("value");
            return this._stringToList(val);
        },

        _setReadOnlyAttr: function (value) {
            this._set("readOnly", value);
            this.textArea.set("readOnly", value);
        },

        // Setter for intermediateChanges
        _setIntermediateChangesAttr: function (value) {
            this.textArea.set("intermediateChanges", value);
            this._set("intermediateChanges", value);
        },

        // Event handler for textarea
        _onTextAreaChanged: function (value) {
            this._setValue(value, false);
        },

        _setValue: function (value, updateTextarea) {
            // Assume value is an array
            var list = value;

            if (typeof value === "string") {
                // Split list
                list = this._stringToList(value);

            } else if (!value) {
                // use empty array for empty value
                list = [];
            }

            if (this._started && epi.areEqual(this.value, list)) {
                return;
            }

            // set value to this widget (and notify observers)
            this._set("value", list);

            // set value to textarea
            updateTextarea && this.textArea.set("value", list.join("\n"));

            if (this._started && this.validate()) {
                // Trigger change event
                this.onChange(list);
            }
        },

        // Convert a string to a list
        _stringToList: function (value) {

            // Return empty array for
            if (!value || typeof value !== "string") {
                return [];
            }

            // Trim whitespace at start and end
            var trimmed = value.replace(/^\s+|\s+$/g, "");

            // Trim whitespace around each linebreak
            var trimmedLines = trimmed.replace(/(\s*\n+\s*)/g, "\n");

            // Split into list
            var list = trimmedLines.split("\n");

            return list;
        }
    });
});
