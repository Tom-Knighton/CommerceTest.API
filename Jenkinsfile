pipeline {
  agent any

  stages {
    stage("Clean") {
        steps {
            script {
                withDotNet(sdk: '.NET 9') {
                    echo "Cleaning project..."
                    dotnetClean sdk: '.NET 9'
                }
            }
        }
    }

    stage("Add source") {
        steps {
            script {
                withDotNet(sdk: '.NET 9') {
                    withCredentials([string(credentialsId: 'addSourceCmd', variable: 'addSourceCmd')]) {
                        try {
                            echo "Adding private sources"
                            sh '$addSourceCmd'
                        } catch (err) {
                            echo "Source already exists"
                        }
                    }
                }
            }
        }
    }

    stage("Restore Project") {
        steps {
            script {
                withDotNet(sdk: '.NET 9') {
                    echo "Restoring Project"
                    dotnetRestore sdk: '.NET 9'
                }
            }
        }
    }

    stage("Unit Tests") {
        steps {
            script {
                withDotNet(sdk: '.NET 9') {
                    echo "Running Unit Tests"
                    dotnetTest sdk: '.NET 9'
                }
            }
        }
    }

    stage ("Build") {
        steps {
            script {
                withDotNet(sdk: '.NET 9') {
                    echo "Building..."
                    dotnetBuild configuration: 'Release', noRestore: true, sdk: '.NET 9'
                }
            }
        }
    }

    stage ("Docker") {
        steps {
            script {
                echo "Building docker image"
                if (env.BRANCH_NAME == "main") {
                    sh "docker build -t commercetestapi:latest -f Dockerfile.prod . --progress=plain"
                    sh "docker save -o commercetestapi_prod.tar commercetestapi:latest"
                }

                if (env.BRANCH_NAME == "develop") {
                    sh "docker build -t commercetestapidev:latest -f Dockerfile.dev . --progress=plain"
                    sh "docker save -o commercetestapi_dev.tar commercetestapidev:latest"
                }
            }
        }
    }

    stage("Publish") {
        steps {
            script {
                if (env.BRANCH_NAME == 'main') {
                    sshPublisher(
                        publishers: [
                            sshPublisherDesc(
                                configName: 'VPS',
                                verbose: true,
                                transfers: [
                                    sshTransfer(
                                        sourceFiles: "commercetestapi_prod.tar",
                                        remoteDirectory: 'CommerceTest.API.Live',
                                        execTimeout: 600000,
                                        execCommand: './_scripts/commercetestapi.sh'
                                    )
                                ]
                            )
                        ]
                    )
                }

                if (env.BRANCH_NAME == 'develop') {
                    sshPublisher(
                        publishers: [
                            sshPublisherDesc(
                                configName: 'VPS',
                                verbose: true,
                                transfers: [
                                    sshTransfer(
                                        sourceFiles: "commercetestapi_dev.tar",
                                        remoteDirectory: 'CommerceTest.API.Dev',
                                        execTimeout: 600000,
                                        execCommand: './_scripts/commercetestapi_dev.sh'
                                    )
                                ]
                            )
                        ]
                    )
                }
            }
        }
    }
  }
}