/*!
 * 
 * Super simple WYSIWYG editor v0.8.20
 * https://summernote.org
 *
 *
 * Copyright 2013- Alan Hong and contributors
 * Summernote may be freely distributed under the MIT license.
 *
 * Date: 2021-10-14T21:15Z
 *
 */
(function webpackUniversalModuleDefinition(root, factory) {
	if(typeof exports === 'object' && typeof module === 'object')
		module.exports = factory();
	else if(typeof define === 'function' && define.amd)
		define([], factory);
	else {
		var a = factory();
		for(var i in a) (typeof exports === 'object' ? exports : root)[i] = a[i];
	}
})(self, function() {
return /******/ (() => { // webpackBootstrap
var __webpack_exports__ = {};
(function ($) {
  $.extend($.summernote.lang, {
    'ar-AR': {
      font: {
        bold: 'Bold',
        italic: 'Italic',
        underline: 'Underline',
        clear: 'Clear formatting',
        height: 'Line height',
        name: 'Font',
        strikethrough: 'Strikethrough',
        subscript: 'Subscript',
        superscript: 'Superscript',
        size: 'Size'
      },
      image: {
        image: 'Image',
        insert: 'Insert image',
        resizeFull: 'Full size',
        resizeHalf: 'Resize to half',
        resizeQuarter: 'Resize to quarter',
        floatLeft: 'Float left',
        floatRight: 'Float right',
        floatNone: 'None',
        shapeRounded: 'Shape: Rounded',
        shapeCircle: 'Shape: Circle',
        shapeThumbnail: 'Shape: Thumbnail',
        shapeNone: 'Shape: None',
        dragImageHere: 'Drag image here',
        dropImage: 'Drop image or text',
        selectFromFiles: 'Select file',
        maximumFileSize: 'Maximum file size',
        maximumFileSizeError: 'Maximum file size exceeded',
        url: 'Image URL',
        remove: 'Remove image',
        original: 'Original'
      },
      video: {
        video: 'Video',
        videoLink: 'Video link',
        insert: 'Insert video',
        url: 'Video URL',
        providers: '(YouTube, Google Drive, Vimeo, Vine, Instagram, DailyMotion or Youku)'
      },
      link: {
        link: 'Link',
        insert: 'Insert',
        unlink: 'Remove link',
        edit: 'Edit',
        textToDisplay: 'Text',
        url: 'Link URL',
        openInNewWindow: 'Open in new window'
      },
      table: {
        table: 'Table',
        addRowAbove: 'Add row above',
        addRowBelow: 'Add row below',
        addColLeft: 'Add column before',
        addColRight: 'Add column after',
        delRow: 'Delete row',
        delCol: 'Delete column',
        delTable: 'Delete table'
      },
      hr: {
        insert: 'Insert horizontal line'
      },
      style: {
        style: 'Style',
        p: 'Normal',
        blockquote: 'Quote',
        pre: 'Code',
        h1: 'Heading 1',
        h2: 'Heading 2',
        h3: 'Heading 3',
        h4: 'Heading 4',
        h5: 'Heading 5',
        h6: 'Heading 6'
      },
      lists: {
        unordered: 'Unordered list',
        ordered: 'Ordered list'
      },
      options: {
        help: 'Help',
        fullscreen: 'Full screen',
        codeview: 'Source code'
      },
      paragraph: {
        paragraph: 'Paragraph',
        outdent: 'Outdent',
        indent: 'Indent',
        left: 'Align left',
        center: 'Center',
        right: 'Align right',
        justify: 'Justify'
      },
      color: {
        recent: 'Recently used',
        more: 'More',
        background: 'Background color',
        foreground: 'Text color',
        transparent: 'Transparent',
        setTransparent: 'No background',
        reset: 'Reset',
        resetToDefault: 'Reset',
        cpSelect: 'Select'
      },
      shortcut: {
        shortcuts: 'Shortcuts',
        close: 'Close',
        textFormatting: 'Text formatting',
        action: 'Action',
        paragraphFormatting: 'Paragraph formatting',
        documentStyle: 'Document style',
        extraKeys: 'Extra keys'
      },
      help: {
        'insertParagraph': 'Insert paragraph',
        'undo': 'Undo last action',
        'redo': 'Redo last action',
        'tab': 'Tab',
        'untab': 'Untab',
        'bold': 'Bold formatting',
        'italic': 'Italic formatting',
        'underline': 'Underline formatting',
        'strikethrough': 'Strikethrough formatting',
        'removeFormat': 'Remove formatting',
        'justifyLeft': 'Align left',
        'justifyCenter': 'Align center',
        'justifyRight': 'Align right',
        'justifyFull': 'Justify',
        'insertUnorderedList': 'Insert unordered list',
        'insertOrderedList': 'Insert ordered list',
        'outdent': 'Outdent current paragraph',
        'indent': 'Indent current paragraph',
        'formatPara': 'Change format of current block to paragraph',
        'formatH1': 'Change format of current block to Heading 1',
        'formatH2': 'Change format of current block to Heading 2',
        'formatH3': 'Change format of current block to Heading 3',
        'formatH4': 'Change format of current block to Heading 4',
        'formatH5': 'Change format of current block to Heading 5',
        'formatH6': 'Change format of current block to Heading 6',
        'insertHorizontalRule': 'Insert horizontal line',
        'linkDialog.show': 'Show link properties'
      },
      history: {
        undo: 'Undo',
        redo: 'Redo'
      },
      specialChar: {
        specialChar: 'Special characters',
        select: 'Select special character'
      }
    }
  });
})(jQuery);
/******/ 	return __webpack_exports__;
/******/ })()
;
});
//# sourceMappingURL=summernote-ar-AR.js.map