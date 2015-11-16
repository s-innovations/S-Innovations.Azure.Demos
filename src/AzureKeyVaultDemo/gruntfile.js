/// <binding AfterBuild='devAfterBuild' />
/*
This file in the main entry point for defining grunt tasks and using grunt plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409
*/
module.exports = function (grunt) {
    grunt.loadNpmTasks('grunt-bower-task');
    grunt.loadNpmTasks('grunt-tsd');
    grunt.loadNpmTasks('grunt-sync');
    grunt.loadNpmTasks('grunt-contrib-less');

    grunt.registerTask('devAfterBuild', ["sync:default","less:default"]);

    grunt.initConfig({
        bower: {
            'install': {
                'options': {
                    'targetDir': 'wwwroot/libs',
                    'verbose': true,
                }
            }
        },
        less: {
            default: {
                options: {
                    compress: true
                },
                files: {
                    "wwwroot/style.min.css": "wwwroot/style.less"
                }
            },
        },
        sync: {
            default: {
                files: [
                    {
                        cwd: "src",
                        src: ["**/content/**/*", "**/templates/**/*"],
                        dest: "wwwroot/libs"
                    }
                ],
                pretend: false,
                verbose: true
            },
        },
        tsd: {
            refresh: {
                options: {
                    command: 'reinstall',
                    latest: true,
                    config: 'tsd.json',
                    opts: {
                    }
                }
            }
        },
    });
};