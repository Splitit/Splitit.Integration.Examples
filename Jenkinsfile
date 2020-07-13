library identifier: 'libs@master', retriever: modernSCM(
  [$class: 'GitSCMSource',
   remote: 'https://github.com/Splitit/jenkins-libs.git',
   credentialsId: '362ed886-91d3-4ae0-b2e8-bb3909e2f2ea']
)

pipeline {
  agent { node { label 'LinuxSlave' } }
  parameters { 
    choice(
      name: 'REQUESTED_ACTION',
      description: 'Type of action to perform',
      choices: [
        'Build',
        'ToDev',
        'ToStg',
        'ToSandbox',
        'ToProd'
      ]
    )
  }

  stages {

    stage('Init') {
      steps {
        echo "${params.REQUESTED_ACTION}"
        checkout scm
        script {
          props = libs.getProps(params.REQUESTED_ACTION, 'splitit.integration.examples')
        }
      }
    }

    stage('Build') {
      when { expression { params.REQUESTED_ACTION == 'Build' } }
      steps {
        script {
          libs.buildPushImage(props, "--no-cache --build-arg BRANCH_NAME=\"${props.git_branch}\"")
        }    
      }
    }

    stage('Test') {
      steps {
        echo 'here should be unitests  and integration tests'
      }
    }

    stage('CheckoutCharts') {
      when { expression { params.REQUESTED_ACTION != 'Build' } }
      steps {
        checkout([$class: 'GitSCM', branches: [[name: 'refs/heads/master']], doGenerateSubmoduleConfigurations: false, extensions: [[$class: 'RelativeTargetDirectory', relativeTargetDir: 'charts'], [$class: 'CloneOption', honorRefspec: true, noTags: false, reference: '', shallow: true]], submoduleCfg: [], userRemoteConfigs: [[credentialsId: '362ed886-91d3-4ae0-b2e8-bb3909e2f2ea', url: 'https://github.com/Splitit/charts.git']]])
      }
    }

    stage('DeployDev') {
      when { expression { params.REQUESTED_ACTION == 'ToDev' } }
      steps {
        script {
          libs.deployHelm(props)
        }
      }
    }

    stage('DeployStage') {
      when { expression { params.REQUESTED_ACTION == 'ToStg' } }
      steps {
        script {
          libs.deployHelm(props)
        }
      }
    }

    stage('DeploySandBox') {
      when { expression { params.REQUESTED_ACTION == 'ToSandbox' } }            
      steps {
        script {
          libs.deployHelm(props)
        }
      }
    }

    stage('DeployProduction') {
      when { expression { params.REQUESTED_ACTION == 'ToProd' } }            
      steps {
        script {
          libs.deployHelm(props)
        }
      }
    }
  }
}
